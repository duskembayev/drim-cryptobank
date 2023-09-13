using cryptobank.api.features.news.config;
using cryptobank.api.features.users.domain;

namespace cryptobank.api.features.news.endpoints.listNews;

public class ListNewsEndpoint : EndpointWithoutRequest<IReadOnlyCollection<NewsModel>>
{
    private readonly IMediator _mediator;
    private readonly IOptions<NewsOptions> _options;

    public ListNewsEndpoint(IMediator mediator, IOptions<NewsOptions> options)
    {
        _mediator = mediator;
        _options = options;
    }

    public override void Configure()
    {
        Get("/news");
        Roles(Role.User, Role.Analyst);
    }

    public override async Task<IReadOnlyCollection<NewsModel>> ExecuteAsync(CancellationToken ct)
    {
        return await _mediator.Send(new ListNewsRequest(_options.Value.ListingCapacity), ct);
    }
}