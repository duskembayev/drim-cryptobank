namespace cryptobank.dal.users;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<int> GetCountAsync(RoleId roleId, CancellationToken cancellationToken);
    Task<int> AddAsync(User user, CancellationToken cancellationToken);
}