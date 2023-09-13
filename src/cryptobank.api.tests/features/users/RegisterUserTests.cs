using cryptobank.api.db;
using cryptobank.api.features.users.domain;
using cryptobank.api.features.users.requests;
using cryptobank.api.tests.extensions;

namespace cryptobank.api.tests.features.users;

public class RegisterUserTests : IClassFixture<ApplicationFixture>
{
    private readonly ApplicationFixture _fixture;
    private readonly HttpClient _client;

    public RegisterUserTests(ApplicationFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.CreateClient();
    }

    [Fact]
    public async Task ShouldRegisterUser()
    {
        var newUserEmail = "new_user@example.com";
        var newUserBirthday = new DateOnly(2000, 3, 26);

        var res = await _client.POSTAsync<RegisterUserRequest, ProblemDetails>("/user/register",
            new RegisterUserRequest
            {
                Email = newUserEmail,
                Password = "newUserP@ssw0rd",
                DateOfBirth = newUserBirthday
            });

        res.ShouldBeOk();

        using var scope = _fixture.AppFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CryptoBankDbContext>();

        var actualUser = await dbContext.Users
            .Include(u => u.Roles)
            .SingleOrDefaultAsync(u => u.Email == newUserEmail);

        actualUser.ShouldNotBeNull();
        actualUser.Email.ShouldBe(newUserEmail);
        actualUser.DateOfBirth.ShouldBe(newUserBirthday);
        actualUser.DateOfRegistration.ShouldBe(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        actualUser.Roles.ShouldBe(new []{ Role.Detached.User });
        actualUser.PasswordHash.ShouldStartWith("$argon2id$v=1");
    }
}