using cryptobank.api.features.accounts.endpoints.transfer;

namespace cryptobank.api.tests.features.accounts.endpoints;

[Collection(AccountsCollection.Name)]
public class TransferRequestValidatorTests
{
    private readonly TransferRequest.Validator _validator = new();

    [Fact]
    public void ShouldValidateAmount()
    {
        var request = new TransferRequest
        {
            Amount = 0
        };

        var result = _validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Amount)
            .WithErrorCode("accounts:transfer:invalid_amount");
    }

    [Fact]
    public void ShouldValidateComment()
    {
        var request = new TransferRequest
        {
            Comment = new string('a', 501)
        };

        var result = _validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Comment)
            .WithErrorCode("accounts:transfer:comment_too_long");
    }
}