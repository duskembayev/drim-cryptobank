using cryptobank.api.db;
using cryptobank.api.features.users.domain;
using cryptobank.api.features.users.endpoints.changeRole;

namespace cryptobank.api.tests.features.users.endpoints;

[Collection(UsersCollection.Name)]
public class ChangeRoleRequestValidatorTests : IAsyncLifetime
{
    private readonly ApplicationFixture _fixture;
    private CryptoBankDbContext? _dbContext;
    private IServiceScope? _scope;
    private ChangeRoleRequest.Validator? _validator;

    public ChangeRoleRequestValidatorTests(ApplicationFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync()
    {
        _scope = _fixture.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<CryptoBankDbContext>();
        _validator = new ChangeRoleRequest.Validator(_dbContext);
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _scope?.Dispose();
        return Task.CompletedTask;
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
        var result = await _validator.TestValidateAsync(new ChangeRoleRequest
        {
            UserId = _fixture.Setup.User.Id,
            Roles = Array.Empty<string>()
        });

        result
            .ShouldHaveValidationErrorFor(request => request.Roles)
            .WithErrorCode("users:change_role:roles_empty");
    }

    [Fact]
    public async Task ShouldValidateRolesInvalid()
    {
        var result = await _validator.TestValidateAsync(new ChangeRoleRequest
        {
            UserId = _fixture.Setup.User.Id,
            Roles = new[] { "Director" }
        });

        result
            .ShouldHaveValidationErrorFor(request => request.Roles)
            .WithErrorCode("users:change_role:roles_invalid");
    }
}