using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace cryptobank.api.redis;

public class RedisConnection : IRedisConnection, IHostedService, IHealthCheck
{
    private const string ConnectionStringName = "redis";

    private readonly IConfiguration _configuration;
    private ConnectionMultiplexer? _connectionMultiplexer;

    public RedisConnection(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
    {
        if (_connectionMultiplexer is { IsConnected: true })
            return Task.FromResult(HealthCheckResult.Healthy());

        return Task.FromResult(HealthCheckResult.Unhealthy());
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var connectionString = _configuration.GetConnectionString(ConnectionStringName)
                               ?? throw new InvalidOperationException(
                                   $"Connection string '{ConnectionStringName}' not found");

        _connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(connectionString);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_connectionMultiplexer != null)
        {
            await _connectionMultiplexer.CloseAsync();
            await _connectionMultiplexer.DisposeAsync();
        }
    }

    public IDatabase Database => _connectionMultiplexer?.GetDatabase()
                                 ?? throw new InvalidOperationException("Redis connection not initialized");

    public IServer Server => _connectionMultiplexer?.GetServers().Single()
                             ?? throw new InvalidOperationException("Redis connection not initialized");
}