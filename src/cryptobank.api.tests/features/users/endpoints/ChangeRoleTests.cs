using cryptobank.api.db;
using cryptobank.api.features.users.domain;
using cryptobank.api.features.users.endpoints.changeRole;
using cryptobank.api.tests.extensions;

namespace cryptobank.api.tests.features.users.endpoints;

[Collection(UsersCollection.Name)]
public class ChangeRoleTests
{
    private readonly HttpClient _client;
    private readonly ApplicationFixture _fixture;

    public ChangeRoleTests(ApplicationFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.HttpClient.CreateClient(_fixture.Setup.Administrator);
    }

    [Fact]
    public async Task ShouldChangeRole()
    {
        var user = await _fixture.Setup.CreateUserAsync(Role.User, "just_user@ex.com");
        var userId = user.Id;

        var res = await _client.POSTAsync<ChangeRoleRequest, ProblemDetails>("/user/changeRole",
            new ChangeRoleRequest
            {
                UserId = userId,
                Roles = new[] {Role.Analyst, Role.User}
            });

        res.ShouldBeOk();
        res.Result.ShouldBeNull();

        using var scope = _fixture.Services.CreateScope();

        user = await scope.ServiceProvider
            .GetRequiredService<CryptoBankDbContext>()
            .Users
            .Include(u => u.Roles)
            .SingleAsync(u => u.Id == userId);

        user.Roles.ShouldContain(Role.Detached.User);
        user.Roles.ShouldContain(Role.Detached.Analyst);
    }
}