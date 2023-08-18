using Enhanced.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace cryptobank.dal.news;

[ContainerEntry(ServiceLifetime.Scoped, typeof(INewsRepository))]
internal sealed class NewsRepository : INewsRepository
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