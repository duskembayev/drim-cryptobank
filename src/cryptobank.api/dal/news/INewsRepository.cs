namespace cryptobank.api.dal.news;

public interface INewsRepository
{
    Task<IReadOnlyList<NewsModel>> ListAsync(int count, CancellationToken cancellationToken = default);
}