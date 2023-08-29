using cryptobank.api.core;
using cryptobank.api.features.users.requests;

namespace cryptobank.api.features.users.endpoints;

public class ChangeRoleEndpoint : Endpoint<ChangeRoleRequest>
{
    private readonly IMediator _mediator;

    public ChangeRoleEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/user/changeRole");
        Roles(ApplicationRole.Administrator);
    }

    public override async Task HandleAsync(ChangeRoleRequest req, CancellationToken ct)
    {
        await _mediator.Send(req, ct);
        await SendOkAsync(ct);
    }
}