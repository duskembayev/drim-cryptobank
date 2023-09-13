using cryptobank.api.features.users.domain;

namespace cryptobank.api.features.accounts.endpoints.createAccount;

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
        Roles(Role.User);
    }

    public override async Task HandleAsync(CreateAccountRequest req, CancellationToken ct)
    {
        var res = await _mediator.Send(req, ct);
        await SendOkAsync(res, ct);
    }
}