using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using cryptobank.api.features.users.config;
using cryptobank.api.redis;
using StackExchange.Redis;
using Attr = cryptobank.api.features.users.services.IRefreshTokenAttributesSerializer.RefreshTokenAttributes;

namespace cryptobank.api.features.users.services;

[Singleton<IRefreshTokenStorage>]
internal sealed class RefreshTokenStorage : IRefreshTokenStorage
{
    private const int LockValueSize = 8;

    private readonly IRefreshTokenAttributesSerializer _attributesSerializer;
    private readonly IOptions<RefreshTokenOptions> _options;
    private readonly IRedisConnection _redisConnection;

    public RefreshTokenStorage(
        IRedisConnection redisConnection,
        IRefreshTokenAttributesSerializer attributesSerializer,
        IOptions<RefreshTokenOptions> options)
    {
        _redisConnection = redisConnection;
        _attributesSerializer = attributesSerializer;
        _options = options;
    }

    public string Issue(int userId, bool allowExtend)
    {
        var token = GenerateToken(userId);

        SetTokenAttr(TokenKey(token), new Attr(userId, allowExtend), _options.Value.Expiration);

        return token;
    }

    public (int UserId, string Token)? Renew(string token)
    {
        var lockKey = LockKey(token);
        var lockValue = RandomValue(LockValueSize);

        if (!_redisConnection.Database.LockTake(lockKey, lockValue, TimeSpan.FromSeconds(10)))
            throw new InvalidOperationException("Failed to acquire lock");

        try
        {
            var lastTokenKey = TokenKey(token);
            var lastTokenTtl = TokenTtl(lastTokenKey);

            if (lastTokenTtl <= TimeSpan.Zero || !TryGetTokenAttr(lastTokenKey, out var lastTokenAttr))
                return null;

            if (lastTokenAttr.IsRevoked)
                return null;

            if (!string.IsNullOrEmpty(lastTokenAttr.ReplacedBy))
            {
                Revoke(token);
                return null;
            }

            var nextToken = GenerateToken(lastTokenAttr.UserId);
            var nextTokenKey = TokenKey(nextToken);

            SetTokenAttr(lastTokenKey, lastTokenAttr with
            {
                ReplacedBy = nextToken
            }, lastTokenTtl);

            SetTokenAttr(nextTokenKey, lastTokenAttr with
            {
                ReplacedBy = null
            }, lastTokenAttr.AllowExtend ? _options.Value.Expiration : lastTokenTtl);

            return (lastTokenAttr.UserId, nextToken);
        }
        finally
        {
            _redisConnection.Database.LockRelease(lockKey, lockValue);
        }
    }

    public void Revoke(string token)
    {
        RevokeCore(TokenKey(token));
    }

    public void RevokeAll(int userId)
    {
        var pageOffset = 0;
        bool canContinue;

        do
        {
            canContinue = false;
            var redisKeys = _redisConnection.Server
                .Keys(pattern: $"rt:u{userId:D5}:*", pageOffset: pageOffset);

            foreach (var redisKey in redisKeys)
            {
                RevokeCore(redisKey);
                canContinue = true;
                pageOffset++;
            }
        } while (canContinue);
    }

    private void RevokeCore([DisallowNull] RedisKey? tokenKey)
    {
        do
        {
            if (!TryGetTokenAttr(tokenKey.Value, out var tokenAttr) || tokenAttr.IsRevoked)
                break;

            SetTokenAttr(tokenKey.Value, tokenAttr with
            {
                IsRevoked = true
            }, TokenTtl(tokenKey.Value));

            tokenKey = !string.IsNullOrEmpty(tokenAttr.ReplacedBy)
                ? TokenKey(tokenAttr.ReplacedBy)
                : (RedisKey?)null;
        } while (tokenKey.HasValue);
    }

    private void SetTokenAttr(RedisKey tokenKey, Attr tokenAttr, TimeSpan expiry)
    {
        var tokenAttrString = _attributesSerializer.Serialize(tokenAttr);

        if (!_redisConnection.Database.StringSet(tokenKey, tokenAttrString, expiry))
            throw new InvalidOperationException("Failed to store token attributes");
    }

    private bool TryGetTokenAttr(RedisKey tokenKey, out Attr tokenAttr)
    {
        string? tokenAttrString = _redisConnection.Database.StringGet(tokenKey);

        if (tokenAttrString is null)
        {
            tokenAttr = default;
            return false;
        }

        tokenAttr = _attributesSerializer.Deserialize(tokenAttrString);
        return true;
    }

    private TimeSpan TokenTtl(RedisKey tokenKey)
    {
        var ttl = _redisConnection.Database.KeyTimeToLive(tokenKey);

        if (ttl is null)
            return TimeSpan.Zero;

        return ttl.Value;
    }

    private static RedisKey TokenKey(string token)
    {
        return $"rt:{token}";
    }

    private static RedisKey LockKey(string token)
    {
        return $"rt:{token}:lock";
    }

    private string GenerateToken(int userId)
    {
        var token = RandomValue(_options.Value.TokenSize);
        return $"u{userId:D5}:{token}";
    }

    private static string RandomValue(int size)
    {
        var buffer = new byte[size];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(buffer);
        return Convert.ToBase64String(buffer);
    }
}