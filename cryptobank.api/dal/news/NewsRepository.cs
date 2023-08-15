using Microsoft.EntityFrameworkCore;

namespace cryptobank.api.dal.news;

public sealed class NewsRepository : INewsRepository
{
    private readonly CryptoBankDbContext _dbContext;

    public NewsRepository(CryptoBankDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IReadOnlyList<NewsModel>> ListAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _dbContext.News
            .OrderByDescending(n => n.Date)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}