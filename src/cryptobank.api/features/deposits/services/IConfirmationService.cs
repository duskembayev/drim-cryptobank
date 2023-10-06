using NBitcoin;

namespace cryptobank.api.features.deposits.services;

public interface IConfirmationService
{
    Task HandleTransactionAddedAsync(uint256 txId);
    Task<bool> HandleTransactionRemovedAsync(uint256 txId);
    Task<IReadOnlySet<uint256>> HandleBlockConnectedAsync(Block block);
    Task<IReadOnlySet<uint256>> HandleBlockDisconnectedAsync(uint256 blockId);
}