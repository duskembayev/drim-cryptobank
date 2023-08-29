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
            .FirstAsync(u => u.Id == request.UserId, cancellationToken);

        user.Roles.Clear();

        foreach (var roleName in request.Roles)
        {
            var role = Role.Detached.GetByName(roleName);
            user.Roles.Add(role);
            _dbContext.AttachRange(role);
        }

        _dbContext.Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}