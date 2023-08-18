using MediatR;

namespace cryptobank.api.features.news;

public record ListNewsRequest(int Count) : IRequest<IReadOnlyCollection<NewsModel>>;
