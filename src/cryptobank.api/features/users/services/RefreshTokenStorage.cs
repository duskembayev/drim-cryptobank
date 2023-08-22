using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using cryptobank.api.features.users.config;
using cryptobank.api.redis;
using StackExchange.Redis;

namespace cryptobank.api.features.users.services;

[ContainerEntry(ServiceLifetime.Singleton, typeof(IRefreshTokenStorage))]
internal class RefreshTokenStorage : IRefreshTokenStorage
{
    private const string AttrIdField = "id";
    private const string AttrExtendField = "ed";
    private const string AttrReplacedByField = "by";
    private const int LockValueSize = 8;

    private readonly IOptions<RefreshTokenOptions> _options;
    private readonly IRedisConnection _redisConnection;

    public RefreshTokenStorage(
        IRedisConnection redisConnection,
        IOptions<RefreshTokenOptions> options)
    {
        _redisConnection = redisConnection;
        _options = options;
    }

    public string Issue(int userId, bool allowExtend)
    {
        var token = GenerateToken(userId);

        SetTokenAttr(TokenKey(token), new JsonObject
        {
            { AttrIdField, userId },
            { AttrExtendField, allowExtend },
            { AttrReplacedByField, null }
        }, _options.Value.Expiration);

        return token;
    }

    public string? Renew(string token)
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

            var userId = lastTokenAttr[AttrIdField]?.GetValue<int?>();
            var extend = lastTokenAttr[AttrExtendField]?.GetValue<bool?>();
            var replacedBy = lastTokenAttr[AttrReplacedByField]?.GetValue<string?>();

            if (userId is null or 0 || extend is null)
                throw new InvalidOperationException("Invalid token attributes format");

            if (!string.IsNullOrEmpty(replacedBy))
            {
                Revoke(token);
                return null;
            }

            var nextToken = GenerateToken(userId.Value);
            var nextTokenKey = TokenKey(nextToken);

            SetTokenAttr(lastTokenKey, new JsonObject
            {
                { AttrIdField, userId.Value },
                { AttrExtendField, extend.Value },
                { AttrReplacedByField, nextToken }
            }, lastTokenTtl);

            SetTokenAttr(nextTokenKey, new JsonObject
            {
                { AttrIdField, userId.Value },
                { AttrExtendField, extend.Value },
                { AttrReplacedByField, null }
            }, extend.Value ? _options.Value.Expiration : lastTokenTtl);

            return nextToken;
        }
        finally
        {
            _redisConnection.Database.LockRelease(lockKey, lockValue);
        }
    }

    public void Revoke(string token)
    {
        var tokenToRevoke = token;

        do
        {
            var tokenToRevokeKey = TokenKey(tokenToRevoke);

            if (!TryDeleteTokenAttr(tokenToRevokeKey, out var tokenToRevokeAttr))
                break;

            tokenToRevoke = tokenToRevokeAttr[AttrReplacedByField]?.GetValue<string?>();
        } while (!string.IsNullOrEmpty(tokenToRevoke));
    }

    public void RevokeAll(int userId)
    {
        bool canContinue;
        
        do
        {
            canContinue = false;
            var redisKeys = _redisConnection.Server.Keys(pattern: $"rt:u{userId:D5}:*");

            foreach (var redisKey in redisKeys)
            {
                Revoke(redisKey.ToString());
                canContinue = true;
            }
        } while (canContinue);
    }

    private void SetTokenAttr(RedisKey tokenKey, JsonNode tokenAttr, TimeSpan expiry)
    {
        var tokenAttrString = tokenAttr.ToJsonString();

        if (!_redisConnection.Database.StringSet(tokenKey, tokenAttrString, expiry))
            throw new InvalidOperationException("Failed to store token attributes");
    }

    private bool TryGetTokenAttr(RedisKey tokenKey, [NotNullWhen(true)] out JsonNode? tokenAttr)
    {
        string? tokenAttrString = _redisConnection.Database.StringGet(tokenKey);

        if (tokenAttrString is null)
        {
            tokenAttr = null;
            return false;
        }

        tokenAttr = JsonNode.Parse(tokenAttrString)
                    ?? throw new InvalidOperationException("Failed to parse token attributes");
        return true;
    }

    private bool TryDeleteTokenAttr(RedisKey tokenKey, [NotNullWhen(true)] out JsonNode? tokenAttr)
    {
        string? tokenAttrString = _redisConnection.Database.StringGetDelete(tokenKey);

        if (tokenAttrString is null)
        {
            tokenAttr = null;
            return false;
        }

        tokenAttr = JsonNode.Parse(tokenAttrString)
                    ?? throw new InvalidOperationException("Failed to parse token attributes");
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