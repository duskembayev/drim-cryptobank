using cryptobank.api.redis;
using cryptobank.api.tests.harnesses.core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NBitcoin.RPC;

namespace cryptobank.api.tests.harnesses;

internal class RedisHarness : Harness
{
    private const int ContainerPort = 6379;
    private const string Password = "P@$sw0rd";

    private IContainer? _container;
    private ushort? _publicPort;

    protected override void OnConfigure(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                {
                    $"ConnectionStrings:{RedisConnection.ConnectionStringName}",
                    $"localhost:{_publicPort},password={Password}"
                }
            });
        });
    }

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        _container = new ContainerBuilder()
            .WithImage("redis:alpine")
            .WithPortBinding(ContainerPort, true)
            .WithEnvironment("REDIS_PASSWORD", Password)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(ContainerPort))
            .Build();

        await _container.StartAsync(cancellationToken);
        _publicPort = _container.GetMappedPublicPort(ContainerPort);
    }

    protected override async Task OnStopAsync(CancellationToken cancellationToken)
    {
        if (_container is not null)
        {
            await _container.StopAsync(cancellationToken);
            await _container.DisposeAsync();
        }
    }
}