using cryptobank.api.features.deposits.config;
using NBitcoin;

namespace cryptobank.api.features.deposits.services;

[Singleton<IConfirmationService>]
internal class ConfirmationService : IConfirmationService
{
    private readonly Dictionary<uint256, List<BlockInfo>> _blocksByTx = new();
    private readonly IOptions<DepositsOptions> _options;
    private readonly HashSet<uint256> _txPool = new();

    public ConfirmationService(IOptions<DepositsOptions> options)
    {
        _options = options;
    }

    public Task HandleTransactionAddedAsync(uint256 txId)
    {
        _txPool.Add(txId);
        _blocksByTx[txId] = new List<BlockInfo>(3);
        return Task.CompletedTask;
    }

    public Task<bool> HandleTransactionRemovedAsync(uint256 txId)
    {
        if (_txPool.Remove(txId))
        {
            _blocksByTx.Remove(txId);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public Task<IReadOnlySet<uint256>> HandleBlockConnectedAsync(Block block)
    {
        var blockTxs = block.Transactions.Select(t => t.GetHash()).ToHashSet();
        var txIds = _txPool.Intersect(blockTxs).ToHashSet();
        var blockId = block.GetHash()!;
        var blockHeight = block.GetCoinbaseHeight()!.Value;

        foreach (var txId in txIds)
            _blocksByTx[txId].Add(new BlockInfo(blockId, blockHeight));

        var confirmedTxs = new HashSet<uint256>();

        foreach (var (txId, blocks) in _blocksByTx)
            if (blocks.Count == 1
                && blockHeight - blocks[0].Height >= _options.Value.MinConfirmations)
                confirmedTxs.Add(txId);

        RemoveTxsCore(confirmedTxs);
        return Task.FromResult<IReadOnlySet<uint256>>(confirmedTxs);
    }

    public Task<IReadOnlySet<uint256>> HandleBlockDisconnectedAsync(uint256 blockId)
    {
        var failedTxs = new HashSet<uint256>();

        foreach (var (txId, blocks) in _blocksByTx)
            if (blocks.RemoveAll(b => b.BlockId == blockId) > 0
                && blocks.Count == 0)
                failedTxs.Add(txId);

        RemoveTxsCore(failedTxs);
        return Task.FromResult<IReadOnlySet<uint256>>(failedTxs);
    }

    private void RemoveTxsCore(HashSet<uint256> txs)
    {
        _txPool.ExceptWith(txs);

        foreach (var txId in txs)
            _blocksByTx.Remove(txId);
    }

    private readonly record struct BlockInfo(uint256 BlockId, int Height);
}