using cryptobank.api.features.accounts.domain;
using cryptobank.api.features.deposits.config;
using cryptobank.api.features.deposits.domain;
using cryptobank.api.redis;
using NBitcoin;
using StackExchange.Redis;

namespace cryptobank.api.features.deposits.services;

[Scoped<ICryptoAddressGenerator>]
internal class CryptoAddressGenerator : ICryptoAddressGenerator
{
    private readonly CryptoBankDbContext _dbContext;
    private readonly IRedisConnection _redisConnection;
    private readonly IRndBytesGenerator _rndBytesGenerator;
    private readonly IOptions<DepositsOptions> _options;

    public CryptoAddressGenerator(
        CryptoBankDbContext dbContext,
        IRedisConnection redisConnection,
        IRndBytesGenerator rndBytesGenerator,
        IOptions<DepositsOptions> options)
    {
        _dbContext = dbContext;
        _redisConnection = redisConnection;
        _rndBytesGenerator = rndBytesGenerator;
        _options = options;
    }

    public async Task<(int XpubId, uint derivationIndex, string address)> GenerateAsync(Currency currency,
        CancellationToken cancellationToken)
    {
        RedisKey lockKey = $"xpub_derivation:{currency:G}";
        RedisValue lockValue = _rndBytesGenerator.GetAsBase64(16);

        if (!await _redisConnection.Database.LockTakeAsync(lockKey, lockValue, TimeSpan.FromSeconds(10)))
            throw new ApplicationException();

        try
        {
            var xpub = await ResolveXpubAsync(currency, cancellationToken);
            var xpubKey = ExtPubKey.Parse(xpub.Value, _options.Value.Network);

            var derivationIndex = xpub.NextDerivationIndex++;
            var derivedPubKey = xpubKey.Derive(derivationIndex);
            var address = derivedPubKey.PubKey
                .GetAddress(ScriptPubKeyType.Segwit, _options.Value.Network)
                .ToString();

            _dbContext.Update(xpub);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return (xpub.Id, derivationIndex, address);
        }
        finally
        {
            await _redisConnection.Database.LockReleaseAsync(lockKey, lockValue);
        }
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