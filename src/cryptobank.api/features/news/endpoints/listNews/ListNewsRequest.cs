namespace cryptobank.api.features.news.endpoints.listNews;

public record ListNewsRequest(int Count) : IRequest<IReadOnlyCollection<NewsModel>>;