using cryptobank.api.utils.security;

namespace cryptobank.api.features.users.endpoints;

public class ProfileEndpoint : EndpointWithoutRequest<ProfileResponse>
{
    private readonly IMediator _mediator;
    private readonly IUserProvider _userProvider;

    public ProfileEndpoint(IMediator mediator, IUserProvider userProvider)
    {
        _mediator = mediator;
        _userProvider = userProvider;
    }

    public override void Configure()
    {
        Get("/user/profile");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var res = await _mediator.Send(new ProfileRequest {UserId = _userProvider.Id}, ct);
        await SendOkAsync(res, ct);
    }
}