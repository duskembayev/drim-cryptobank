using cryptobank.api.core;

namespace cryptobank.api.features.accounts.endpoints;

public class CreateAccountEndpoint : Endpoint<CreateAccountRequest>
{
    private readonly IMediator _mediator;

    public CreateAccountEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/accounts/create");
        Roles(ApplicationRole.User);
    }

    public override async Task HandleAsync(CreateAccountRequest req, CancellationToken ct)
    {
        var res = await _mediator.Send(req, ct);
        await SendOkAsync(res.AccountId, cancellation: ct);
    }
}