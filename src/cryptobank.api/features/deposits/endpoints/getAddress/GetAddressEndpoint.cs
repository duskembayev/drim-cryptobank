using cryptobank.api.features.users.domain;

namespace cryptobank.api.features.deposits.endpoints.getAddress;

public class GetAddressEndpoint : Endpoint<GetAddressRequest>
{
    private readonly IMediator _mediator;

    public GetAddressEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("deposits/address");
        Roles(Role.User);
    }

    public override async Task HandleAsync(GetAddressRequest req, CancellationToken ct)
    {
        var res = await _mediator.Send(req, ct);
        await SendOkAsync(res, ct);
    }
}