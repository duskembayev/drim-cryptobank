using cryptobank.api.db;
using cryptobank.api.tests.extensions;
using Microsoft.Extensions.Configuration;

namespace cryptobank.api.tests.fixtures;

public class DbFixture : IAsyncLifetime
{
    public DbFixture()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false)
            .AddEnvironmentVariables()
            .Build();

        var options = new DbContextOptionsBuilder<CryptoBankDbContext>()
            .UseNpgsql(configuration.GetConnectionStringWithRndDatabase())
            .Options;

        DbContext = new CryptoBankDbContext(options);
    }

    public CryptoBankDbContext DbContext { get; }

    public async Task InitializeAsync()
    {
        await DbContext.Database.EnsureCreatedAsync();
        await DbContext.ApplyReferencesAsync();
        await DbContext.ApplySamplesAsync();
    }

    public async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();
    }
}