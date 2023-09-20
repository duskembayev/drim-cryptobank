using cryptobank.api.features.accounts.domain;
using cryptobank.api.features.accounts.services;

namespace cryptobank.api.features.accounts.endpoints.transfer;

public class TransferHandler : IRequestHandler<TransferRequest, EmptyResponse>
{
    private readonly CryptoBankDbContext _dbContext;
    private readonly ICurrencyConverter _currencyConverter;
    private readonly ITimeProvider _timeProvider;

    public TransferHandler(
        CryptoBankDbContext dbContext,
        ICurrencyConverter currencyConverter,
        ITimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _currencyConverter = currencyConverter;
        _timeProvider = timeProvider;
    }

    public async Task<EmptyResponse> Handle(TransferRequest request, CancellationToken cancellationToken)
    {
        var sourceAmount = request.Amount;

        var source = await _dbContext.Accounts.SingleAsync(
            a => a.UserId == request.UserId && a.AccountId == request.SourceAccountId,
            cancellationToken);

        var target = await _dbContext.Accounts.SingleAsync(
            a => a.AccountId == request.TargetAccountId,
            cancellationToken);

        if (source.Balance < sourceAmount)
            throw new LogicException("accounts:transfer:insufficient_funds", "Insufficient funds");

        var (targetAmount, rate) = await _currencyConverter
            .ConvertAsync(source.Currency, target.Currency, sourceAmount);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        source.Balance -= sourceAmount;
        target.Balance += targetAmount;

        _dbContext.Add(new InternalTransfer
        {
            SourceUserId = source.UserId,
            SourceAccountId = source.AccountId,
            SourceCurrency = source.Currency,
            SourceAmount = sourceAmount,
            TargetUserId = target.UserId,
            TargetAccountId = target.AccountId,
            TargetCurrency = target.Currency,
            TargetAmount = targetAmount,
            ConversionRate = rate,
            Comment = request.Comment,
            DateOfCreate = _timeProvider.UtcNow
        });

        _dbContext.UpdateRange(source, target);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new EmptyResponse();
    }
}