using Enhanced.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace cryptobank.dal.users;

[ContainerEntry(ServiceLifetime.Scoped, typeof(IUserRepository))]
internal sealed class UserRepository : IUserRepository
{
    private readonly CryptoBankDbContext _dbContext;

    public UserRepository(CryptoBankDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        email = email.ToLowerInvariant();

        return await _dbContext.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<int> GetCountAsync(RoleId roleId, CancellationToken cancellationToken)
    {
        if (roleId == RoleId.None)
            throw new ArgumentOutOfRangeException(nameof(roleId), roleId, "Role cannot be None.");

        var role = (int) roleId;

        return await _dbContext.Users
            .Include(u => u.Roles)
            .Where(u => u.Roles.Any(r => r.Id == role))
            .CountAsync(cancellationToken);
    }

    public Task<int> AddAsync(User user, CancellationToken cancellationToken)
    {
        _dbContext.Roles.AttachRange(user.Roles);
        _dbContext.Users.Add(user);

        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}