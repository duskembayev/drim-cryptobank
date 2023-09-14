using cryptobank.api.features.users.domain;
using cryptobank.api.features.users.services;

namespace cryptobank.api.tests.fixtures;

public static class ApplicationFixtureExtensions
{
    public static void Authorize(this ApplicationFixture fixture, User user)
    {
        fixture.AppFactory.AccessToken = fixture.AppFactory.Services
            .GetRequiredService<IAccessTokenProvider>()
            .Issue(user);
    }
}