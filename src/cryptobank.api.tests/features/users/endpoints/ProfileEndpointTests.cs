using System.Net.Http.Json;
using cryptobank.api.features.users.domain;
using cryptobank.api.features.users.responses;
using Shouldly;

namespace cryptobank.api.tests.features.users.endpoints;

public class CryptoBankFixture : IClassFixture<CryptoBankFixture>
{
    
}

public class ProfileEndpointTests : IAsyncLifetime
{
    private readonly CryptoBankApplicationFactory _appFactory = new();

    private User? _user;
    private HttpClient? _client;

    [Fact]
    public async Task ShouldReturnUserProfile()
    {
        _client.ShouldNotBeNull();
        _user.ShouldNotBeNull();

        var res = await _client.GetFromJsonAsync<ProfileResponse>("/user/profile");

        res.ShouldNotBeNull();
        res.Email.ShouldBe(_user.Email);
        res.Roles.ShouldBe(_user.Roles.Select(role => role.Name));
        res.DateOfBirth.ShouldBe(_user.DateOfBirth);
        res.DateOfRegistration.ShouldBe(_user.DateOfRegistration);
    }

    [Fact]
    public async Task ShouldReturnUserProfile2()
    {
        _client.ShouldNotBeNull();
        _user.ShouldNotBeNull();

        var res = await _client.GetFromJsonAsync<ProfileResponse>("/user/profile");

        res.ShouldNotBeNull();
        res.Email.ShouldBe(_user.Email);
        res.Roles.ShouldBe(_user.Roles.Select(role => role.Name));
        res.DateOfBirth.ShouldBe(_user.DateOfBirth);
        res.DateOfRegistration.ShouldBe(_user.DateOfRegistration);
    }

    public async Task InitializeAsync()
    {
        _user = await _appFactory.InitializeWithAuthorizationAsync();
        _client = _appFactory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await _appFactory.DisposeAsync();
    }
}