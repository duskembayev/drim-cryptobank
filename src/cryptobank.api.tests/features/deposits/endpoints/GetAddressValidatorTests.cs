using cryptobank.api.features.deposits.endpoints.getAddress;

namespace cryptobank.api.tests.features.deposits.endpoints;

public class GetAddressValidatorTests
{
    private readonly GetAddressRequest.Validator _validator = new();

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ShouldValidateAccount(string accountId)
    {
        var result = _validator.TestValidate(new GetAddressRequest
        {
            AccountId = accountId
        });

        result
            .ShouldHaveValidationErrorFor(r => r.AccountId)
            .WithErrorCode("deposits:get_address:account_id_empty");
    }
}