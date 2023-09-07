using System.Net.Http.Headers;
using cryptobank.api.features.users.services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace cryptobank.api.tests;

internal sealed class CryptoBankApplicationFactory : WebApplicationFactory<Program>
{
    public string? AccessToken { get; set; }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
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