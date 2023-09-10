using System.Net.Http.Headers;
using cryptobank.api.features.users.services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace cryptobank.api.tests.fixtures;

internal sealed class CryptoBankApplicationFactory : WebApplicationFactory<Program>
{
    public string? AccessToken { get; set; }
    public string? Database { get; init; }

    public Task InitializeAsync()
    {
        _ = Server;
        return Task.CompletedTask;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        if (Database is not null)
        {
            builder.ConfigureAppConfiguration((context, configurationBuilder) =>
            {
                var npgConnString = context.Configuration["ConnectionStrings:postgres"];
                var npgConnStringBuilder = new NpgsqlConnectionStringBuilder(npgConnString)
                {
                    Database = Database
                };
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:postgres"] = npgConnStringBuilder.ConnectionString
                });
            });
        }

        builder.UseEnvironment(Environments.Development);
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);

        if (AccessToken is not null)
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(AccessTokenConstants.Bearer, AccessToken);
    }

    public static CryptoBankApplicationFactory CreateWithRndDatabase()
    {
        return new CryptoBankApplicationFactory
        {
            Database = "db_" + Guid.NewGuid().ToString("N")
        };
    }
}