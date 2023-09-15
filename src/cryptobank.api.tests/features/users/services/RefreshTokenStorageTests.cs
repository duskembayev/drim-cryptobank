using System.Text.Json.Nodes;
using cryptobank.api.features.users.config;
using cryptobank.api.features.users.services;
using cryptobank.api.utils.environment;

namespace cryptobank.api.tests.features.users.services;

public class RefreshTokenStorageTests : IClassFixture<RedisFixture>
{
    private readonly RedisFixture _fixture;
    private readonly RefreshTokenStorage _storage;

    public RefreshTokenStorageTests(RedisFixture fixture)
    {
        _fixture = fixture;

        var options = new RefreshTokenOptions
        {
            TokenSize = 8,
            Expiration = TimeSpan.FromMinutes(1)
        };

        _storage = new RefreshTokenStorage(
            _fixture.Connection,
            new RefreshTokenAttributesSerializer(),
            new StubRndBytesGenerator(),
            new OptionsWrapper<RefreshTokenOptions>(options));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ShouldIssueToken(bool allowExtend)
    {
        var token = _storage.Issue(19, allowExtend);

        token.ShouldBe("u00019:TlwFaaJAER0=");

        var redisValue = _fixture.Connection.Database.StringGet("rt:u00019:TlwFaaJAER0=");
        var node = JsonNode.Parse(redisValue.ToString());

        node.ShouldNotBeNull();

        node["id"]?.GetValue<int>().ShouldBe(19);
        node["ed"]?.GetValue<bool>().ShouldBe(allowExtend);
        node["by"]?.GetValue<string>().ShouldBeNull();
        node["rv"]?.GetValue<bool>().ShouldBeFalse();
    }

    private class StubRndBytesGenerator : IRndBytesGenerator
    {
        public byte[] GetBytes(int count) => throw new NotSupportedException();

        public void Fill(Span<byte> span)
        {
            span[0] = 78;
            span[1] = 92;
            span[2] = 5;
            span[3] = 105;
            span[4] = 162;
            span[5] = 64;
            span[6] = 17;
            span[7] = 29;
        }
    }
}