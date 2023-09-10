using System.Net;
using cryptobank.api.db;
using cryptobank.api.features.users.requests;
using cryptobank.api.tests.extensions;
using cryptobank.api.tests.fixtures;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

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
        var actualUser = await dbContext.Users.SingleOrDefaultAsync(u => u.Email == newUserEmail);

        actualUser.ShouldNotBeNull();
        actualUser.DateOfBirth.ShouldBe(newUserBirthday);
    }

    [Fact]
    public async Task ShouldReturnErrorWhenEmailAlreadyExists()
    {
        var res = await _client.POSTAsync<RegisterUserRequest, ProblemDetails>("/user/register",
            new RegisterUserRequest
            {
                Email = ApplicationFixture.UserEmail,
                Password = "newUserP@ssw0rd",
                DateOfBirth = new DateOnly(2000, 3, 26)
            });

        res.ShouldBeProblem(HttpStatusCode.Conflict, "users:register:user_exists");
    }

    [Theory]
    [InlineData("email")]
    [InlineData("email@")]
    [InlineData("@domain")]
    [InlineData("domain.com")]
    public async Task ShouldReturnErrorWhenInvalidEmail(string email)
    {
        var res = await _client.POSTAsync<RegisterUserRequest, ProblemDetails>("/user/register",
            new RegisterUserRequest
            {
                Email = email,
                Password = "newUserP@ssw0rd",
                DateOfBirth = new DateOnly(2000, 3, 26)
            });

        res.ShouldBeValidationProblem(HttpStatusCode.BadRequest, "email", "users:register:email_invalid");
    }

    [Theory]
    [InlineData(10)]
    [InlineData(14)]
    [InlineData(15)]
    public async Task ShouldReturnErrorWhenDayOfBirthIsInvalid(int age)
    {
        var res = await _client.POSTAsync<RegisterUserRequest, ProblemDetails>("/user/register",
            new RegisterUserRequest
            {
                Email = "newUser@example.com",
                Password = "newUserP@ssw0rd",
                DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-age))
            }
        );

        res.ShouldBeValidationProblem(HttpStatusCode.BadRequest, "dateOfBirth", "users:register:too_young");
    }
}