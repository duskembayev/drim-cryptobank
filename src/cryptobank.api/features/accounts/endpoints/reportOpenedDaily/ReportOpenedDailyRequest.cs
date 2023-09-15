using Microsoft.AspNetCore.Mvc;

namespace cryptobank.api.features.accounts.endpoints.reportOpenedDaily;

public class ReportOpenedDailyRequest : IRequest<IReadOnlyCollection<OpenedDailyModel>>
{
    [FromQuery] public DateOnly Start { get; set; }
    [FromQuery] public DateOnly End { get; set; }

    public class Validator : AbstractValidator<ReportOpenedDailyRequest>
    {
        public Validator()
        {
            RuleFor(request => request.End)
                .GreaterThanOrEqualTo(request => request.Start)
                .WithErrorCode("accounts:reports:opened_daily:invalid_date_range")
                .WithMessage("Invalid date range");
        }
    }
}