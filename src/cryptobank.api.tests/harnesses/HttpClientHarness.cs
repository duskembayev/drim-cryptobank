using System.Net.Http.Headers;
using cryptobank.api.features.users.domain;
using cryptobank.api.features.users.services;
using cryptobank.api.tests.harnesses.core;

namespace cryptobank.api.tests.harnesses;

internal class HttpClientHarness : Harness
{
    public HttpClient CreateClient()
    {
        return Factory.CreateClient();
    }

    public HttpClient CreateClient(User user)
    {
        var accessToken = Factory.Services
            .GetRequiredService<IAccessTokenProvider>()
            .Issue(user);

        var client = Factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(AccessTokenConstants.Bearer, accessToken);

        return client;
    }
}