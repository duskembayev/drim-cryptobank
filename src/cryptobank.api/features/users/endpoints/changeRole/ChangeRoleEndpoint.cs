using cryptobank.api.features.users.domain;

namespace cryptobank.api.features.users.endpoints.changeRole;

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
        Roles(Role.Administrator);
    }

    public override async Task HandleAsync(ChangeRoleRequest req, CancellationToken ct)
    {
        await _mediator.Send(req, ct);
        await SendOkAsync(ct);
    }
}