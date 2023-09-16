using System.Text.Json.Nodes;
using cryptobank.api.features.users.config;
using cryptobank.api.features.users.services;
using cryptobank.api.utils.environment;
using StackExchange.Redis;

namespace cryptobank.api.tests.features.users.services;

public class RefreshTokenStorageTests : IClassFixture<RedisFixture>
{
    private readonly RedisFixture _fixture;
    private readonly StubRndBytesGenerator _rndBytesGenerator;
    private readonly RefreshTokenStorage _storage;

    public RefreshTokenStorageTests(RedisFixture fixture)
    {
        _fixture = fixture;
        _rndBytesGenerator = new StubRndBytesGenerator();

        var options = new RefreshTokenOptions
        {
            TokenSize = 8,
            Expiration = TimeSpan.FromMinutes(1)
        };

        _storage = new RefreshTokenStorage(
            _fixture.Connection,
            new RefreshTokenAttributesSerializer(),
            _rndBytesGenerator,
            new OptionsWrapper<RefreshTokenOptions>(options));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ShouldIssueToken(bool allowExtend)
    {
        const int userId = 17;
        const string tokenPayload = "TlwFaaJAER0=";

        _rndBytesGenerator.NextToken = tokenPayload;
        var token = _storage.Issue(userId, allowExtend);

        token.ShouldBe($"u000{userId}:{tokenPayload}");
        AssertRedisValue(userId, tokenPayload, allowExtend);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ShouldRenewToken(bool allowExtend)
    {
        const int userId = 18;
        const string initialToken = "TlwFaaJAER0=";
        const string nextToken = "TlRFaRJaBr0=";

        _rndBytesGenerator.NextToken = initialToken;
        var token = _storage.Issue(userId, allowExtend);

        _rndBytesGenerator.NextToken = nextToken;
        var renewResult = _storage.Renew(token);

        renewResult.ShouldNotBeNull();
        renewResult.Value.UserId.ShouldBe(userId);
        renewResult.Value.Token.ShouldBe($"u000{userId}:{nextToken}");

        AssertRedisValue(userId, initialToken, allowExtend, nextToken);
        AssertRedisValue(userId, nextToken, allowExtend);
    }

    [Fact]
    public void ShouldRevokeToken()
    {
        const int userId = 19;
        const string tokenPayload = "TlwFaaJAER0=";

        _rndBytesGenerator.NextToken = tokenPayload;
        var token = _storage.Issue(userId, false);
        _storage.Revoke(token);

        AssertRedisValue(userId, tokenPayload, false, isRevoked: true);
    }

    [Fact]
    public void ShouldNotRenewTokenWhenExpired()
    {
        const int userId = 20;
        const string tokenPayload = "TlwFaaKAER0=";

        _rndBytesGenerator.NextToken = tokenPayload;
        var token = _storage.Issue(userId, false);

        _fixture.Connection.Database
            .KeyExpire(RedisKey(userId, tokenPayload), TimeSpan.Zero)
            .ShouldBeTrue();

        _storage.Renew(token).ShouldBeNull();
    }

    [Fact]
    public void ShouldNotRenewTokenWhenRevoked()
    {
        const int userId = 21;
        const string tokenPayload = "TlwFaaKAER0=";

        _rndBytesGenerator.NextToken = tokenPayload;
        var token = _storage.Issue(userId, false);
        _storage.Revoke(token);

        _storage.Renew(token).ShouldBeNull();
    }

    [Fact]
    public void ShouldNotRenewTokenWhenAlreadyRenewed()
    {
        const int userId = 22;
        const string initialToken = "TlwFaaJAER0=";
        const string nextToken = "TlRFaRJaBr0=";

        _rndBytesGenerator.NextToken = initialToken;
        var token = _storage.Issue(userId, false);

        _rndBytesGenerator.NextToken = nextToken;
        var renewResult = _storage.Renew(token);
        var duplicatedRenewResult = _storage.Renew(token);

        renewResult.ShouldNotBeNull();
        duplicatedRenewResult.ShouldBeNull();

        AssertRedisValue(userId, initialToken, false, nextToken, true);
        AssertRedisValue(userId, nextToken, false, isRevoked: true);
    }

    [Fact]
    public void ShouldRevokeAllUsersTokens()
    {
        const int userId = 22;
        const string userToken1 = "AlwFaaJAER0=";
        const string userToken2 = "BlRFaRJaBr0=";

        const int anotherUserId = 23;
        const string anotherToken = "CuNFaRJaBr0=";

        _rndBytesGenerator.NextToken = userToken1;
        _storage.Issue(userId, false);
        _rndBytesGenerator.NextToken = anotherToken;
        _storage.Issue(anotherUserId, false);
        _rndBytesGenerator.NextToken = userToken2;
        _storage.Issue(userId, false);

        _storage.RevokeAll(userId);

        AssertRedisValue(userId, userToken1, false, isRevoked: true);
        AssertRedisValue(userId, userToken2, false, isRevoked: true);
        AssertRedisValue(anotherUserId, anotherToken, false, isRevoked: false);
    }

    private void AssertRedisValue(int userId, string token, bool allowExtend,
        string? replacedBy = null, bool isRevoked = false)
    {
        var redisValue = _fixture.Connection.Database.StringGet(RedisKey(userId, token));
        var node = JsonNode.Parse(redisValue.ToString());

        node.ShouldNotBeNull();

        node["id"]?.GetValue<int>().ShouldBe(userId);
        node["ed"]?.GetValue<bool>().ShouldBe(allowExtend);
        node["by"]?.GetValue<string>().ShouldBe(replacedBy != null ? $"u000{userId}:{replacedBy}" : null);
        node["rv"]?.GetValue<bool>().ShouldBe(isRevoked);
    }

    private static RedisKey RedisKey(int userId, string token)
    {
        return $"rt:u000{userId}:{token}";
    }

    private class StubRndBytesGenerator : IRndBytesGenerator
    {
        public string? NextToken { get; set; }

        public byte[] GetBytes(int count)
        {
            throw new NotSupportedException();
        }

        public void Fill(Span<byte> span)
        {
            if (NextToken is null)
                throw new InvalidOperationException("NextToken is not set");

            Convert.FromBase64String(NextToken).CopyTo(span);
        }
    }
}