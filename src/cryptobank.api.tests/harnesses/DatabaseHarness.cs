using cryptobank.api.db;
using cryptobank.api.tests.harnesses.core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace cryptobank.api.tests.harnesses;

internal class DatabaseHarness : Harness
{
    private readonly PostgresHarness _postgresHarness;

    public DatabaseHarness(PostgresHarness postgresHarness)
    {
        _postgresHarness = postgresHarness;
    }

    public async Task ExecuteAsync(Func<CryptoBankDbContext, Task> action)
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CryptoBankDbContext>();
        await action(dbContext);
    }

    public async Task<T> ExecuteAsync<T>(Func<CryptoBankDbContext, Task<T>> action)
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CryptoBankDbContext>();
        return await action(dbContext);
    }

    protected override void OnConfigure(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"Database:RestoreEnabled", "false"}
            });
        });
    }

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        _postgresHarness.ThrowIfNotStarted();

        await using var scope = Factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CryptoBankDbContext>();
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        await dbContext.ApplyReferencesAsync();
    }
}