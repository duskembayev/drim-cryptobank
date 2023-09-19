using cryptobank.api.features.accounts.services;

namespace cryptobank.api.features.accounts.endpoints.transfer;

public class TransferHandler : IRequestHandler<TransferRequest, EmptyResponse>
{
    private readonly CryptoBankDbContext _dbContext;
    private readonly ICurrencyConverter _currencyConverter;

    public TransferHandler(CryptoBankDbContext dbContext, ICurrencyConverter currencyConverter)
    {
        _dbContext = dbContext;
        _currencyConverter = currencyConverter;
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

        var targetAmount = await _currencyConverter.ConvertAsync(source.Currency, target.Currency, sourceAmount);
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        source.Balance -= sourceAmount;
        target.Balance += targetAmount;

        _dbContext.UpdateRange(source, target);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new EmptyResponse();
    }
}