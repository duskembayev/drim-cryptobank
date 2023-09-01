using cryptobank.api.features.users.requests;
using cryptobank.api.features.users.responses;

namespace cryptobank.api.features.users.endpoints;

public class LoginUserEndpoint : Endpoint<LoginUserRequest, TokenResponse>
{
    private readonly IMediator _mediator;

    public LoginUserEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/user/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginUserRequest req, CancellationToken ct)
    {
        var res = await _mediator.Send(req, ct);
        await SendOkAsync(res, ct);
    }
}