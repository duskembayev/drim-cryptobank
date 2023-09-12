using System.Net;
using cryptobank.api.features.users.requests;
using cryptobank.api.features.users.responses;
using cryptobank.api.tests.extensions;
using FastEndpoints;
using Shouldly;

namespace cryptobank.api.tests.features.users;

public class LoginUserTests : IClassFixture<ApplicationFixture>
{
    private readonly HttpClient _client;

    public LoginUserTests(ApplicationFixture fixture)
    {
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task ShouldLogin()
    {
        var res = await _client.POSTAsync<LoginUserRequest, TokenResponse>("/user/login",
            new LoginUserRequest
            {
                Email = ApplicationFixture.UserEmail,
                Password = ApplicationFixture.UserPassword
            });

        res.ShouldBeOk();
        res.Result.ShouldNotBeNull();
        res.Result.AccessToken.ShouldNotBeNullOrWhiteSpace();
        res.Result.RefreshToken.ShouldNotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData("invaliduser@example.com", "inva@lidP@ssw0rd")]
    [InlineData(ApplicationFixture.UserEmail, "inva@lidP@ssw0rd")]
    [InlineData("invaliduser@example.com", ApplicationFixture.UserPassword)]
    public async Task ShouldReturnErrorWhenInvalidCredentials(string email, string password)
    {
        var res = await _client.POSTAsync<LoginUserRequest, ProblemDetails>("/user/login",
            new LoginUserRequest
            {
                Email = email,
                Password = password
            });

        res.ShouldBeProblem(HttpStatusCode.BadRequest, "users:login:invalid_credentials");
    }
}