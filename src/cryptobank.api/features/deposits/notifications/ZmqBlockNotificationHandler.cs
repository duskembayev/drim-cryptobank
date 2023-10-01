using cryptobank.api.features.deposits.config;
using cryptobank.api.features.deposits.domain;
using cryptobank.api.features.deposits.services;
using NBitcoin;

namespace cryptobank.api.features.deposits.notifications;

internal class ZmqBlockNotificationHandler : INotificationHandler<ZmqBlockNotification>
{
    private readonly IConfirmationService _confirmationService;
    private readonly CryptoBankDbContext _dbContext;
    private readonly ILogger<ZmqBlockNotificationHandler> _logger;
    private readonly IOptions<DepositsOptions> _options;
    private readonly ITimeProvider _timeProvider;

    public ZmqBlockNotificationHandler(
        IConfirmationService confirmationService,
        CryptoBankDbContext dbContext,
        ITimeProvider timeProvider,
        IOptions<DepositsOptions> options,
        ILogger<ZmqBlockNotificationHandler> logger)
    {
        _confirmationService = confirmationService;
        _dbContext = dbContext;
        _timeProvider = timeProvider;
        _options = options;
        _logger = logger;
    }

    public async Task Handle(ZmqBlockNotification notification, CancellationToken cancellationToken)
    {
        var block = notification.ToBlock(_options.Value.Network);
        var txIds = await _confirmationService.HandleBlockConnectedAsync(block);

        if (txIds.Count == 0)
            return;

        var utcNow = _timeProvider.UtcNow;

        foreach (var txId in txIds)
            await ConfirmTransactionAsync(txId, utcNow);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task ConfirmTransactionAsync(uint256 txId, DateTime dateOfCompletion)
    {
        var txIdString = txId.ToString();

        var count = await _dbContext.CryptoDeposits
            .Where(d => d.TxId == txIdString && d.Status == DepositStatus.Pending)
            .ExecuteUpdateAsync(calls => calls
                .SetProperty(d => d.Status, DepositStatus.Confirmed)
                .SetProperty(d => d.DateOfCompletion, dateOfCompletion));

        if (count == 0)
            _logger.LogWarning("Transaction {TxId} was not found in the database", txIdString);
        else
            _logger.LogInformation("Transaction {TxId} was marked as confirmed", txIdString);
    }
}