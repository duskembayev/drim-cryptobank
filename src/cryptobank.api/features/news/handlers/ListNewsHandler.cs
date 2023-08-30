using cryptobank.api.features.news.models;
using cryptobank.api.features.news.requests;

namespace cryptobank.api.features.news.handlers;

public class ListNewsHandler : IRequestHandler<ListNewsRequest, IReadOnlyCollection<NewsModel>>
{
    private readonly CryptoBankDbContext _dbContext;

    public ListNewsHandler(CryptoBankDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<NewsModel>> Handle(
        ListNewsRequest request,
        CancellationToken cancellationToken)
    {
        return await _dbContext.News
            .OrderByDescending(n => n.Date)
            .Select(n => new NewsModel(n.Title, n.Content, n.Date, n.Author))
            .Take(request.Count)
            .ToListAsync(cancellationToken);
    }
}