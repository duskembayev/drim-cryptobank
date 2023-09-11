using cryptobank.api.features.accounts.models;
using cryptobank.api.features.accounts.requests;

namespace cryptobank.api.features.accounts.handlers;

public class ListAccountsHandler : IRequestHandler<ListAccountsRequest, IReadOnlyCollection<AccountModel>>
{
    private readonly CryptoBankDbContext _dbContext;

    public ListAccountsHandler(CryptoBankDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<AccountModel>> Handle(
        ListAccountsRequest request,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Accounts
            .Where(a => a.UserId == request.UserId)
            .Select(a => new AccountModel(a.AccountId, a.Currency, a.Balance))
            .ToListAsync(cancellationToken);
    }
}