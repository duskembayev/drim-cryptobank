using cryptobank.api.db;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            .Take(request.Count)
            .Select(n => new NewsModel(n.Title, n.Content, n.Date, n.Author))
            .ToListAsync(cancellationToken: cancellationToken);
    }
}