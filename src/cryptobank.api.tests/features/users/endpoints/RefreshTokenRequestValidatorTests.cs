using cryptobank.api.features.users.endpoints.refreshToken;

namespace cryptobank.api.tests.features.users.endpoints;

[Collection(UsersCollection.Name)]
public class RefreshTokenRequestValidatorTests
{
    private readonly RefreshTokenRequest.Validator _validator = new();

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ShouldValidateRefreshToken(string refreshToken)
    {
        var result = _validator.TestValidate(new RefreshTokenRequest
        {
            RefreshToken = refreshToken
        });

        result
            .ShouldHaveValidationErrorFor(request => request.RefreshToken)
            .WithErrorCode("users:refresh_token:token_empty");
    }
}