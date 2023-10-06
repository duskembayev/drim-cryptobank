using cryptobank.api.features.deposits.config;
using cryptobank.api.features.deposits.domain;
using cryptobank.api.features.deposits.services;
using NBitcoin;

namespace cryptobank.api.features.deposits.notifications;

internal class ZmqTransactionNotificationHandler : INotificationHandler<ZmqTransactionNotification>
{
    private readonly IConfirmationService _confirmationService;
    private readonly CryptoBankDbContext _dbContext;
    private readonly ITimeProvider _timeProvider;
    private readonly IOptions<DepositsOptions> _options;

    public ZmqTransactionNotificationHandler(
        IConfirmationService confirmationService,
        CryptoBankDbContext dbContext,
        ITimeProvider timeProvider,
        IOptions<DepositsOptions> options)
    {
        _confirmationService = confirmationService;
        _dbContext = dbContext;
        _timeProvider = timeProvider;
        _options = options;
    }

    public async Task Handle(ZmqTransactionNotification notification, CancellationToken cancellationToken)
    {
        var transaction = notification.ToTransaction(_options.Value.Network);

        if (!await TryHandleDeposits(transaction, cancellationToken))
            return;

        await _confirmationService.HandleTransactionAddedAsync(transaction.GetHash());
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<bool> TryHandleDeposits(Transaction transaction, CancellationToken cancellationToken)
    {
        var txId = transaction.GetHash().ToString()!;

        if (await _dbContext.CryptoDeposits.AnyAsync(
                d => d.TxId == txId && d.Status == DepositStatus.Pending,
                cancellationToken))
            return false;

        var isHandled = false;

        foreach (var txOut in transaction.Outputs)
        {
            var cryptoAddress = txOut.ScriptPubKey
                .GetDestinationAddress(_options.Value.Network)
                ?.ToString();

            if (cryptoAddress is null)
                continue;

            var depositAddress = await _dbContext.DepositAddresses
                .SingleOrDefaultAsync(d => d.CryptoAddress == cryptoAddress, cancellationToken);

            if (depositAddress is null)
                continue;

            _dbContext.CryptoDeposits.Add(new CryptoDeposit
            {
                AccountId = depositAddress.AccountId,
                TxId = txId,
                Amount = txOut.Value.ToDecimal(MoneyUnit.BTC),
                DateOfCreation = _timeProvider.UtcNow
            });

            isHandled = true;
        }

        return isHandled;
    }
}