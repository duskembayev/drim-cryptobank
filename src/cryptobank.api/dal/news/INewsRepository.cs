namespace cryptobank.api.dal.news;

public interface INewsRepository
{
    Task<IReadOnlyList<News>> ListAsync(int count, CancellationToken cancellationToken = default);
}