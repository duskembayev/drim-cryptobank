using cryptobank.api.features.accounts.config;
using cryptobank.api.features.accounts.domain;
using cryptobank.api.features.accounts.services;
using cryptobank.api.utils.environment;

namespace cryptobank.api.features.accounts.handlers;

public class CreateAccountHandler : IRequestHandler<CreateAccountRequest, Account>
{
    private readonly CryptoBankDbContext _dbContext;
    private readonly IAccountIdGenerator _accountIdGenerator;
    private readonly ITimeProvider _timeProvider;
    private readonly IOptions<AccountsOptions> _options;

    public CreateAccountHandler(
        CryptoBankDbContext dbContext,
        IAccountIdGenerator accountIdGenerator,
        ITimeProvider timeProvider,
        IOptions<AccountsOptions> options)
    {
        _dbContext = dbContext;
        _accountIdGenerator = accountIdGenerator;
        _timeProvider = timeProvider;
        _options = options;
    }

    public async Task<Account> Handle(CreateAccountRequest request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .Include(user => user.Accounts)
            .FirstOrDefaultAsync(user => user.Id == request.UserId, cancellationToken);
        
        if (user is null)
            throw new ApplicationException("User not found");

        if (user.Accounts.Count >= _options.Value.MaxAccountsPerUser)
            throw new ApplicationException("User has reached the maximum number of accounts");

        var account = new Account
        {
            User = user,
            Currency = request.Currency,
            DateOfOpening = _timeProvider.UtcNow,
            AccountId = _accountIdGenerator.GenerateAccountId(),
            Balance = 0
        };

        await _dbContext.Accounts.AddAsync(account, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return account;
    }
}