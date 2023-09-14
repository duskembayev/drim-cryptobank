using cryptobank.api.features.accounts.domain;
using cryptobank.api.features.accounts.endpoints.openAccount;

namespace cryptobank.api.tests.features.accounts.endpoints;

public class OpenAccountRequestValidatorTests
{
    private readonly OpenAccountRequest.Validator _validator = new();

    [Theory]
    [InlineData((Currency)99)]
    public void ShouldValidateCurrency(Currency currency)
    {
        var result = _validator.TestValidate(new OpenAccountRequest
        {
            Currency = currency,
            UserId = 3
        });

        result
            .ShouldHaveValidationErrorFor(request => request.Currency)
            .WithErrorCode(GeneralErrorCodes.InvalidEnumValue);
    }
}