using cryptobank.api.features.deposits.domain;
using cryptobank.api.features.deposits.services;
using NBitcoin;

namespace cryptobank.api.features.deposits.notifications;

internal class ZmqSequenceNotificationHandler : INotificationHandler<ZmqSequenceNotification>
{
    private readonly IConfirmationService _confirmationService;
    private readonly CryptoBankDbContext _dbContext;
    private readonly ITimeProvider _timeProvider;
    private readonly ILogger<ZmqSequenceNotificationHandler> _logger;

    public ZmqSequenceNotificationHandler(
        IConfirmationService confirmationService,
        CryptoBankDbContext dbContext,
        ITimeProvider timeProvider,
        ILogger<ZmqSequenceNotificationHandler> logger)
    {
        _confirmationService = confirmationService;
        _dbContext = dbContext;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public Task Handle(ZmqSequenceNotification notification, CancellationToken cancellationToken)
    {
        var messageType = notification.GetMessageType();
        var hash = notification.GetHash();

        return messageType switch
        {
            ZmqSequenceNotification.MessageType.BlockhashDisconnected => HandleBlockDisconnectedAsync(hash),
            ZmqSequenceNotification.MessageType.TransactionHashRemoved => HandleTransactionRemovedAsync(hash),
            _ => Task.CompletedTask
        };
    }

    private async Task HandleBlockDisconnectedAsync(uint256 hash)
    {
        var txIds = await _confirmationService.HandleBlockDisconnectedAsync(hash);

        if (txIds.Count == 0)
            return;

        var utcNow = _timeProvider.UtcNow;

        foreach (var txId in txIds)
            await FailTransactionAsync(txId, utcNow);

        await _dbContext.SaveChangesAsync();
    }

    private async Task HandleTransactionRemovedAsync(uint256 txId)
    {
        if (await _confirmationService.HandleTransactionRemovedAsync(txId))
            return;

        await FailTransactionAsync(txId, _timeProvider.UtcNow);
        await _dbContext.SaveChangesAsync();
    }

    private async Task FailTransactionAsync(uint256 txId, DateTime dateOfCompletion)
    {
        var txIdString = txId.ToString();

        var count = await _dbContext.CryptoDeposits
            .Where(d => d.TxId == txIdString && d.Status == DepositStatus.Pending)
            .ExecuteUpdateAsync(calls => calls
                .SetProperty(d => d.Status, DepositStatus.Failed)
                .SetProperty(d => d.DateOfCompletion, dateOfCompletion));

        if (count == 0)
            _logger.LogWarning("Transaction {TxId} was not found in the database", txIdString);
        else
            _logger.LogInformation("Transaction {TxId} was marked as failed", txIdString);
    }
}