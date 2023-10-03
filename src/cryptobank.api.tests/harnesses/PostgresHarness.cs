using cryptobank.api.db;
using cryptobank.api.tests.harnesses.core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace cryptobank.api.tests.harnesses;

internal class PostgresHarness : Harness
{
    private const int ContainerPort = 5432;
    private const string Database = "cryptobank";
    private const string Username = "postgres";
    private const string Password = "P@$sw0rd";

    private IContainer? _container;
    private ushort? _publicPort;

    protected override void OnConfigure(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            var connectionString = GetConnectionString();
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                {$"ConnectionStrings:{DbConstants.ConnectionStringName}", connectionString}
            });
        });
    }

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        _container = new ContainerBuilder()
            .WithImage("postgres:alpine")
            .WithPortBinding(ContainerPort, true)
            .WithEnvironment("POSTGRES_DB", Database)
            .WithEnvironment("POSTGRES_USER", Username)
            .WithEnvironment("POSTGRES_PASSWORD", Password)
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

    private string GetConnectionString()
    {
        Assert.NotNull(_container);
        Assert.NotNull(_publicPort);

        return new NpgsqlConnectionStringBuilder
        {
            Host = "localhost",
            Database = Database,
            Username = Username,
            Password = Password,
            Port = _publicPort.Value,
            MaxPoolSize = 10,
            ConnectionIdleLifetime = 60
        }.ConnectionString;
    }
}