using cryptobank.api.features.users.requests;

namespace cryptobank.api.features.users.endpoints;

public class ProfileEndpoint : Endpoint<ProfileRequest>
{
    private readonly IMediator _mediator;

    public ProfileEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/user/profile");
    }

    public override async Task HandleAsync(ProfileRequest req, CancellationToken ct)
    {
        var res = await _mediator.Send(req, ct);
        await SendOkAsync(res, ct);
    }
}