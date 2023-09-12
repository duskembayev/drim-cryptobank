using cryptobank.api.db;
using cryptobank.api.features.users.domain;
using cryptobank.api.features.users.requests;
using Microsoft.EntityFrameworkCore;

namespace cryptobank.api.tests.features.users;

public class ChangeRoleRequestValidatorTests : IClassFixture<DbFixture>
{
    private readonly ChangeRoleRequest.Validator _validator;
    private readonly CryptoBankDbContext _dbContext;

    public ChangeRoleRequestValidatorTests(DbFixture fixture)
    {
        _dbContext = fixture.DbContext;
        _validator = new ChangeRoleRequest.Validator(_dbContext);
    }

    [Theory]
    [InlineData(-5)]
    [InlineData(0)]
    [InlineData(105)]
    public async Task ShouldValidateUserExists(int invalidUserId)
    {
        var result = await _validator.TestValidateAsync(new ChangeRoleRequest
        {
            UserId = invalidUserId,
            Roles = new[] { Role.Analyst }
        });

        result
            .ShouldHaveValidationErrorFor(request => request.UserId)
            .WithErrorCode(GeneralErrorCodes.InvalidUser);
    }

    [Fact]
    public async Task ShouldValidateRolesEmpty()
    {
        var user = await _dbContext.Users.SingleAsync(user => user.Email == "fry@futurama.com");

        var result = await _validator.TestValidateAsync(new ChangeRoleRequest
        {
            UserId = user.Id,
            Roles = Array.Empty<string>()
        });

        result
            .ShouldHaveValidationErrorFor(request => request.Roles)
            .WithErrorCode("users:change_role:roles_empty");
    }

    [Fact]
    public async Task ShouldValidateRolesInvalid()
    {
        var user = await _dbContext.Users.SingleAsync(user => user.Email == "fry@futurama.com");

        var result = await _validator.TestValidateAsync(new ChangeRoleRequest
        {
            UserId = user.Id,
            Roles = new[] { "Director" }
        });

        result
            .ShouldHaveValidationErrorFor(request => request.Roles)
            .WithErrorCode("users:change_role:roles_invalid");
    }
}