using cryptobank.api.config;
using cryptobank.api.utils.environment;
using cryptobank.api.utils.security;
using cryptobank.dal;
using cryptobank.dal.users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace cryptobank.api.features.users.handlers;

public class RegisterUserHandler : IRequestHandler<RegisterUserRequest, User>
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

    public async Task<User> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.ToLowerInvariant();

        if (await _dbContext.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
            throw new ApplicationException("User already exists");

        var role = await GetRoleAsync(request, cancellationToken);

        var passwordSalt = _passwordHashAlgorithm
            .GenerateSalt(_options.Value.PasswordSaltSize);
        var passwordHash = _passwordHashAlgorithm
            .ComputeHash(request.Password, passwordSalt, _options.Value.PasswordHashSize);

        var user = new User
        {
            Email = email,
            Role = role,
            PasswordSalt = passwordSalt,
            PasswordHash = passwordHash,
            DateOfBirth = request.DateOfBirth,
            DateOfRegistration = _timeProvider.UtcNow
        };

        _dbContext.AttachRange(user.Roles);

        await _dbContext.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return user;
    }

    private async Task<RoleId> GetRoleAsync(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        if (request.Email.Equals(_options.Value.FallbackAdminEmail, StringComparison.OrdinalIgnoreCase))
        {
            const int adminRoleId = (int) RoleId.Administrator;

            if (await _dbContext.Users
                    .Include(user => user.Roles)
                    .AnyAsync(user => user.Roles.Any(role => role.Id == adminRoleId), cancellationToken))
                return RoleId.Administrator;
        }

        return RoleId.User;
    }
}