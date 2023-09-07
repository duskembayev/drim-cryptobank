using Microsoft.AspNetCore.Mvc.Testing;

namespace cryptobank.api.tests.features.users.services;

public class RefreshTokenStorageTests : IAsyncLifetime
{
    private readonly CryptoBankApplicationFactory _appFactory = new();

    public async Task InitializeAsync()
    {
        await _appFactory.InitializeWithAuthorizationAsync();
        _ = _appFactory.Server;
    }

    public async Task DisposeAsync()
    {
        await _appFactory.DisposeAsync();
    }
}