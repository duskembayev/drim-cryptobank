using cryptobank.api.config;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Options;

namespace cryptobank.api.features.news.endpoints;

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
        AllowAnonymous();
    }

    public override async Task<IReadOnlyCollection<NewsModel>> ExecuteAsync(CancellationToken ct)
    {
        return await _mediator.Send(new ListNewsRequest(_options.Value.ListingCapacity), ct);
    }
}