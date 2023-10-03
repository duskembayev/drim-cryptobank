using cryptobank.api.features.users.endpoints.loginUser;

namespace cryptobank.api.tests.features.users.endpoints;

[Collection(UsersCollection.Name)]
public class LoginUserRequestValidatorTests
{
    private readonly LoginUserRequest.Validator _validator = new();

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ShouldValidateEmail(string email)
    {
        var result = _validator.TestValidate(new LoginUserRequest
        {
            Email = email,
            Password = "P@$sw0rd"
        });

        result
            .ShouldHaveValidationErrorFor(request => request.Email)
            .WithErrorCode("users:login:email_empty");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ShouldValidatePassword(string password)
    {
        var result = _validator.TestValidate(new LoginUserRequest
        {
            Email = "user@example.com",
            Password = password
        });

        result
            .ShouldHaveValidationErrorFor(request => request.Password)
            .WithErrorCode("users:login:password_empty");
    }
}