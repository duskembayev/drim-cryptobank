using cryptobank.api.features.accounts.endpoints.reportOpenedDaily;

namespace cryptobank.api.tests.features.accounts.endpoints;

[Collection(AccountsCollection.Name)]
public class ReportOpenedDailyRequestValidatorTests
{
    private readonly ReportOpenedDailyRequest.Validator _validator = new();

    [Fact]
    public void ShouldFailWhenEndLessThanStart()
    {
        var result = _validator.TestValidate(new ReportOpenedDailyRequest
        {
            Start = new DateOnly(2020, 1, 5),
            End = new DateOnly(2020, 1, 3)
        });

        result
            .ShouldHaveValidationErrorFor(request => request.End)
            .WithErrorCode("accounts:reports:opened_daily:invalid_date_range");
    }

    [Fact]
    public void ShouldSuccessWhenEndEqualToStart()
    {
        var result = _validator.TestValidate(new ReportOpenedDailyRequest
        {
            Start = new DateOnly(2020, 1, 5),
            End = new DateOnly(2020, 1, 5)
        });

        result.ShouldNotHaveAnyValidationErrors();
    }
}