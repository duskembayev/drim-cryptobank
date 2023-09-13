using cryptobank.api.features.users.domain;

namespace cryptobank.api.features.accounts.endpoints.listAccounts;

public class ListAccountsEndpoint : Endpoint<ListAccountsRequest>
{
    private readonly IMediator _mediator;

    public ListAccountsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/accounts");
        Roles(Role.User);
    }

    public override async Task HandleAsync(ListAccountsRequest req, CancellationToken ct)
    {
        var res = await _mediator.Send(req, ct);
        await SendOkAsync(res, ct);
    }
}