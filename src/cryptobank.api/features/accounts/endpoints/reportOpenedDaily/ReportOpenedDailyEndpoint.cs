using cryptobank.api.features.users.domain;

namespace cryptobank.api.features.accounts.endpoints.reportOpenedDaily;

public class ReportOpenedDailyEndpoint : Endpoint<ReportOpenedDailyRequest>
{
    private readonly IMediator _mediator;

    public ReportOpenedDailyEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/accounts/reports/openedDaily");
        Roles(Role.Analyst);
    }

    public override async Task HandleAsync(ReportOpenedDailyRequest req, CancellationToken ct)
    {
        var res = await _mediator.Send(req, ct);
        await SendOkAsync(res, ct);
    }
}