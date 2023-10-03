using System.Data;
using cryptobank.api.features.accounts.domain;
using cryptobank.api.features.deposits.config;
using cryptobank.api.features.deposits.domain;
using NBitcoin;

namespace cryptobank.api.features.deposits.services;

[Scoped<ICryptoAddressGenerator>]
internal class CryptoAddressGenerator : ICryptoAddressGenerator
{
    private readonly CryptoBankDbContext _dbContext;
    private readonly IOptions<DepositsOptions> _options;

    public CryptoAddressGenerator(
        CryptoBankDbContext dbContext,
        IOptions<DepositsOptions> options)
    {
        _dbContext = dbContext;
        _options = options;
    }

    public async Task<(int XpubId, uint derivationIndex, string address)> GenerateAsync(Currency currency,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _dbContext.Database
            .BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken: cancellationToken);

        var xpub = await ResolveXpubAsync(currency, cancellationToken);
        var xpubKey = ExtPubKey.Parse(xpub.Value, _options.Value.Network);

        var derivationIndex = xpub.NextDerivationIndex++;
        var derivedPubKey = xpubKey.Derive(derivationIndex);
        var address = derivedPubKey.PubKey
            .GetAddress(ScriptPubKeyType.Segwit, _options.Value.Network)
            .ToString();

        _dbContext.Update(xpub);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return (xpub.Id, derivationIndex, address);
    }

    private async ValueTask<Xpub> ResolveXpubAsync(Currency currency, CancellationToken cancellationToken)
    {
        var xpub = await _dbContext.Xpubs
            .SingleOrDefaultAsync(x => x.Currency == currency && x.IsActive, cancellationToken);

        if (xpub is null)
        {
            xpub = new Xpub
            {
                Currency = currency,
                Value = _options.Value.Xpub
            };

            _dbContext.Add(xpub);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return xpub;
    }
}