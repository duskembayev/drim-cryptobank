using Enhanced.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace cryptobank.api.dal.news;

[ContainerEntry(ServiceLifetime.Scoped, typeof(INewsRepository))]
public sealed class NewsRepository : INewsRepository
{
    private readonly CryptoBankDbContext _dbContext;

    public NewsRepository(CryptoBankDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IReadOnlyList<News>> ListAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _dbContext.News
            .OrderByDescending(n => n.Date)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}