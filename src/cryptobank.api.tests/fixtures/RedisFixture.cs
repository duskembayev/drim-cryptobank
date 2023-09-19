using cryptobank.api.redis;
using Microsoft.Extensions.Configuration;

namespace cryptobank.api.tests.fixtures;

public class RedisFixture : IAsyncLifetime
{
    private readonly RedisConnection _connection;

    public RedisFixture()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false)
            .AddEnvironmentVariables()
            .Build();

        _connection = new RedisConnection(configuration);
    }

    public IRedisConnection Connection => _connection;

    public async Task InitializeAsync()
    {
        await _connection.StartAsync(default);
    }

    public async Task DisposeAsync()
    {
        await _connection.StopAsync(default);
    }
}