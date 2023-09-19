using cryptobank.api.features.users.domain;

namespace cryptobank.api.features.accounts.endpoints.transfer;

public class TransferEndpoint : Endpoint<TransferRequest>
{
    private readonly IMediator _mediator;

    public TransferEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public override void Configure()
    {
        Post("/accounts/transfer");
        Roles(Role.User);
    }

    public override async Task HandleAsync(TransferRequest req, CancellationToken ct)
    {
        await _mediator.Send(req, ct);
        await SendOkAsync(ct);
    }
}