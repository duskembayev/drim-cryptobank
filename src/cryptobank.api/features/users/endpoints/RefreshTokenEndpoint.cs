using cryptobank.api.features.users.requests;
using cryptobank.api.features.users.responses;

namespace cryptobank.api.features.users.endpoints;

public class RefreshTokenEndpoint : Endpoint<RefreshTokenRequest, TokenResponse>
{
    private readonly IMediator _mediator;

    public RefreshTokenEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/user/refreshToken");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RefreshTokenRequest req, CancellationToken ct)
    {
        var res = await _mediator.Send(req, ct);
        await SendOkAsync(res, ct);
    }
}