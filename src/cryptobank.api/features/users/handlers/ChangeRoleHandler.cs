using cryptobank.api.features.users.domain;
using cryptobank.api.features.users.requests;

namespace cryptobank.api.features.users.handlers;

public class ChangeRoleHandler : IRequestHandler<ChangeRoleRequest>
{
    private readonly CryptoBankDbContext _dbContext;

    public ChangeRoleHandler(CryptoBankDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(ChangeRoleRequest request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .Include(u => u.Roles)
            .SingleAsync(u => u.Id == request.UserId, cancellationToken);

        var newRoles = _dbContext.Roles
            .ToList()
            .Where(r => request.Roles.Contains(r.Name, StringComparer.OrdinalIgnoreCase));

        user.Roles.Clear();
        foreach (var role in newRoles) user.Roles.Add(role);

        _dbContext.Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}