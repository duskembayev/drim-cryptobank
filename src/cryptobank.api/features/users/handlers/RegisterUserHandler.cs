using cryptobank.api.core;
using cryptobank.api.features.users.config;
using cryptobank.api.features.users.domain;
using cryptobank.api.utils.environment;
using cryptobank.api.utils.security;

namespace cryptobank.api.features.users.handlers;

public class RegisterUserHandler : IRequestHandler<RegisterUserRequest, int>
{
    private readonly IPasswordHashAlgorithm _passwordHashAlgorithm;
    private readonly ITimeProvider _timeProvider;
    private readonly CryptoBankDbContext _dbContext;
    private readonly IOptions<RegisterUserOptions> _options;

    public RegisterUserHandler(
        CryptoBankDbContext dbContext,
        IPasswordHashAlgorithm passwordHashAlgorithm,
        ITimeProvider timeProvider,
        IOptions<RegisterUserOptions> options)
    {
        _passwordHashAlgorithm = passwordHashAlgorithm;
        _timeProvider = timeProvider;
        _dbContext = dbContext;
        _options = options;
    }

    public async Task<int> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.ToLowerInvariant();

        if (await _dbContext.Users.AnyAsync(u => u.Email == email, cancellationToken))
            throw new ApplicationException("User already exists");

        var role = await GetRoleAsync(email, cancellationToken);
        var passwordHash = await _passwordHashAlgorithm.HashAsync(request.Password);

        var user = new User
        {
            Email = email,
            PasswordHash = passwordHash,
            DateOfBirth = request.DateOfBirth,
            DateOfRegistration = _timeProvider.UtcNow,
            Roles = {role}
        };

        _dbContext.AttachRange(user.Roles);
        await _dbContext.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return user.Id;
    }

    private async Task<Role> GetRoleAsync(string email, CancellationToken cancellationToken)
    {
        if (email.Equals(_options.Value.FallbackAdminEmail, StringComparison.OrdinalIgnoreCase)
            && !await _dbContext.Users
                .Include(user => user.Roles)
                .AnyAsync(user => user.Roles.Any(role => role.Id == ApplicationRole.AdministratorRoleId), cancellationToken))
            return Role.Detached.Administrator;

        return Role.Detached.User;
    }
}