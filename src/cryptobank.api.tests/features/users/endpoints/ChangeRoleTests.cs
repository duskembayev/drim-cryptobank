using cryptobank.api.db;
using cryptobank.api.features.users.domain;
using cryptobank.api.features.users.endpoints.changeRole;
using cryptobank.api.tests.extensions;

namespace cryptobank.api.tests.features.users.endpoints;

public class ChangeRoleTests : IClassFixture<ApplicationFixture>
{
    private readonly HttpClient _client;
    private readonly ApplicationFixture _fixture;

    public ChangeRoleTests(ApplicationFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient(_fixture.Administrator);
    }

    [Fact]
    public async Task ShouldChangeRole()
    {
        var userId = _fixture.User.Id;

        var res = await _client.POSTAsync<ChangeRoleRequest, ProblemDetails>("/user/changeRole",
            new ChangeRoleRequest
            {
                UserId = userId,
                Roles = new[] {Role.Analyst, Role.User}
            });

        res.ShouldBeOk();
        res.Result.ShouldBeNull();

        using var scope = _fixture.AppFactory.Services.CreateScope();

        var user = await scope.ServiceProvider
            .GetRequiredService<CryptoBankDbContext>()
            .Users
            .Include(u => u.Roles)
            .SingleAsync(u => u.Id == userId);

        user.Roles.ShouldContain(Role.Detached.User);
        user.Roles.ShouldContain(Role.Detached.Analyst);
    }
}