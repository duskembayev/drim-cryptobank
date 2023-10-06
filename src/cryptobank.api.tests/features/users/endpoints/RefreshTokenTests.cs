using System.Net.Http.Headers;
using cryptobank.api.features.users.endpoints.getProfile;
using cryptobank.api.features.users.endpoints.loginUser;
using cryptobank.api.features.users.endpoints.refreshToken;
using cryptobank.api.features.users.models;
using cryptobank.api.features.users.services;
using cryptobank.api.tests.extensions;
using cryptobank.api.tests.harnesses;

namespace cryptobank.api.tests.features.users.endpoints;

[Collection(UsersCollection.Name)]
public class RefreshTokenTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly ApplicationFixture _fixture;
    private TokenModel? _token;

    public RefreshTokenTests(ApplicationFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.HttpClient.CreateClient();
    }

    public async Task InitializeAsync()
    {
        var result = await _client.POSTAsync<LoginUserRequest, TokenModel>("/user/login", new LoginUserRequest
        {
            Email = SetupHarness.UserEmail,
            Password = SetupHarness.UserPassword
        });

        _token = result.Result;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task ShouldRefreshToken()
    {
        var result = await _client.POSTAsync<RefreshTokenRequest, TokenModel>("/user/refreshToken",
            new RefreshTokenRequest
            {
                RefreshToken = _token!.RefreshToken
            });

        result.ShouldBeOk();
        result.Result.ShouldNotBeNull();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(AccessTokenConstants.Bearer, result.Result.AccessToken);

        var profileResult = await _client.GETAsync<EmptyRequest, ProfileModel>("/user/profile", new EmptyRequest());
        profileResult.ShouldBeOk();
    }

    [Fact]
    public async Task ShouldNotRefreshTokenWhenRevoked()
    {
        var refreshTokenStorage = _fixture.Services.GetRequiredService<IRefreshTokenStorage>();
        refreshTokenStorage.Revoke(_token!.RefreshToken);

        var result = await _client.POSTAsync<RefreshTokenRequest, ProblemDetails>("/user/refreshToken",
            new RefreshTokenRequest
            {
                RefreshToken = _token.RefreshToken
            });

        result.ShouldBeProblem(HttpStatusCode.BadRequest, "users:refresh_token:expired");
    }
}