using System.Security;
using cryptobank.api.features.users.services;
using cryptobank.api.utils.security;

namespace cryptobank.api.features.users.handlers;

public class LoginUserHandler : IRequestHandler<LoginUserRequest, TokenResponse>
{
    private readonly IAccessTokenProvider _accessTokenProvider;
    private readonly CryptoBankDbContext _dbContext;
    private readonly IPasswordHashAlgorithm _passwordHashAlgorithm;

    public LoginUserHandler(
        CryptoBankDbContext dbContext,
        IPasswordHashAlgorithm passwordHashAlgorithm,
        IAccessTokenProvider accessTokenProvider)
    {
        _dbContext = dbContext;
        _passwordHashAlgorithm = passwordHashAlgorithm;
        _accessTokenProvider = accessTokenProvider;
    }

    public async Task<TokenResponse> Handle(LoginUserRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.ToLowerInvariant();

        var user = await _dbContext.Users
            .Include(user => user.Roles)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user is null)
            throw new SecurityException("Invalid credentials");

        if (!_passwordHashAlgorithm.ValidateHash(request.Password, user.PasswordHash, user.PasswordSalt))
            throw new SecurityException("Invalid credentials");

        return new TokenResponse
        {
            AccessToken = await _accessTokenProvider.IssueAsync(user, cancellationToken)
        };
    }
}