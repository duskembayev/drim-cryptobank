using System.Net.Http.Headers;
using cryptobank.api.features.users.services;
using cryptobank.api.tests.extensions;
using cryptobank.api.utils.exchange;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace cryptobank.api.tests.fixtures;

internal sealed class CryptoBankApplicationFactory : WebApplicationFactory<Program>
{
    public string? AccessToken { get; set; }

    public Task InitializeAsync()
    {
        _ = Server;
        return Task.CompletedTask;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, configurationBuilder) =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["WARMUP_TIMEOUT"] = "0",
                ["Features:Accounts:MaxAccountsPerUser"] = "2",
                ["ConnectionStrings:postgres"] = context.Configuration.GetConnectionStringWithRndDatabase()
            });
        });

        builder.ConfigureServices(collection =>
        {
            collection.AddSingleton<IExchangeRateSource, ExchangeRateSourceMock>();
        });

        builder.UseEnvironment(Environments.Development);
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);

        if (AccessToken is not null)
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(AccessTokenConstants.Bearer, AccessToken);
    }
}