using cryptobank.api.features.users.endpoints.loginUser;
using cryptobank.api.features.users.models;
using cryptobank.api.tests.extensions;
using cryptobank.api.tests.harnesses;

namespace cryptobank.api.tests.features.users.endpoints;

[Collection(UsersCollection.Name)]
public class LoginUserTests
{
    private readonly HttpClient _client;

    public LoginUserTests(ApplicationFixture fixture)
    {
        _client = fixture.HttpClient.CreateClient();
    }

    [Fact]
    public async Task ShouldLogin()
    {
        var res = await _client.POSTAsync<LoginUserRequest, TokenModel>("/user/login",
            new LoginUserRequest
            {
                Email = SetupHarness.UserEmail,
                Password = SetupHarness.UserPassword
            });

        res.ShouldBeOk();
        res.Result.ShouldNotBeNull();
        res.Result.AccessToken.ShouldNotBeNullOrWhiteSpace();
        res.Result.RefreshToken.ShouldNotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData("invaliduser@example.com", "inva@lidP@ssw0rd")]
    [InlineData(SetupHarness.UserEmail, "inva@lidP@ssw0rd")]
    [InlineData("invaliduser@example.com", SetupHarness.UserPassword)]
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