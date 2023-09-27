using cryptobank.api.features.accounts.domain;
using cryptobank.api.features.deposits.domain;
using cryptobank.api.features.deposits.services;

namespace cryptobank.api.features.deposits.endpoints.getAddress;

public class GetAddressHandler : IRequestHandler<GetAddressRequest, DepositAddressModel>
{
    private readonly ICryptoAddressGenerator _cryptoAddressGenerator;
    private readonly CryptoBankDbContext _dbContext;

    public GetAddressHandler(ICryptoAddressGenerator cryptoAddressGenerator, CryptoBankDbContext dbContext)
    {
        _cryptoAddressGenerator = cryptoAddressGenerator;
        _dbContext = dbContext;
    }

    public async Task<DepositAddressModel> Handle(GetAddressRequest request, CancellationToken cancellationToken)
    {
        var depositAddress = await _dbContext.DepositAddresses
            .SingleOrDefaultAsync(a => a.AccountId == request.AccountId, cancellationToken);

        if (depositAddress is null)
            depositAddress = await CreateDepositAddressAsync(request, cancellationToken);

        return new DepositAddressModel(depositAddress.CryptoAddress);
    }

    private async ValueTask<DepositAddress> CreateDepositAddressAsync(
        GetAddressRequest request,
        CancellationToken cancellationToken)
    {
        var account = await _dbContext.Accounts
            .SingleOrDefaultAsync(
                a => a.UserId == request.UserId && a.AccountId == request.AccountId,
                cancellationToken);

        if (account is null)
            throw new LogicException("deposits:get_address:account_not_found", "Account not found");

        if (account is not {Currency: Currency.BTC})
            throw new LogicException("deposits:get_address:account_not_btc", "Account is not BTC");

        var (xpubId, derivationIndex, address) = await _cryptoAddressGenerator
            .GenerateAsync(account.Currency, cancellationToken);

        var depositAddress = new DepositAddress
        {
            XpubId = xpubId,
            AccountId = request.AccountId,
            DerivationIndex = derivationIndex,
            CryptoAddress = address
        };

        _dbContext.DepositAddresses.Add(depositAddress);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return depositAddress;
    }
}