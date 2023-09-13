using cryptobank.api.features.users.domain;

namespace cryptobank.api.features.accounts.endpoints.openAccount;

public class OpenAccountEndpoint : Endpoint<OpenAccountRequest>
{
    private readonly IMediator _mediator;

    public OpenAccountEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/accounts/open");
        Roles(Role.User);
    }

    public override async Task HandleAsync(OpenAccountRequest req, CancellationToken ct)
    {
        var res = await _mediator.Send(req, ct);
        await SendOkAsync(res, ct);
    }
}