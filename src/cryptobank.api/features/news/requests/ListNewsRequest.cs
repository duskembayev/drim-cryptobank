using cryptobank.api.features.news.models;

namespace cryptobank.api.features.news.requests;

public record ListNewsRequest(int Count) : IRequest<IReadOnlyCollection<NewsModel>>;