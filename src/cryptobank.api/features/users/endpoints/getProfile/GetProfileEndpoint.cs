namespace cryptobank.api.features.users.endpoints.getProfile;

public class GetProfileEndpoint : Endpoint<GetProfileRequest>
{
    private readonly IMediator _mediator;

    public GetProfileEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/user/profile");
    }

    public override async Task HandleAsync(GetProfileRequest req, CancellationToken ct)
    {
        var res = await _mediator.Send(req, ct);
        await SendOkAsync(res, ct);
    }
}