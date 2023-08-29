using System.Security;
using cryptobank.api.features.users.requests;
using cryptobank.api.features.users.responses;
using cryptobank.api.features.users.services;
using cryptobank.api.utils.security;

namespace cryptobank.api.features.users.handlers;

public class LoginUserHandler : IRequestHandler<LoginUserRequest, TokenResponse>
{
    private readonly IAccessTokenProvider _accessTokenProvider;
    private readonly CryptoBankDbContext _dbContext;
    private readonly IPasswordHashAlgorithm _passwordHashAlgorithm;
    private readonly IRefreshTokenStorage _refreshTokenStorage;

    public LoginUserHandler(
        CryptoBankDbContext dbContext,
        IPasswordHashAlgorithm passwordHashAlgorithm,
        IAccessTokenProvider accessTokenProvider,
        IRefreshTokenStorage refreshTokenStorage)
    {
        _dbContext = dbContext;
        _passwordHashAlgorithm = passwordHashAlgorithm;
        _accessTokenProvider = accessTokenProvider;
        _refreshTokenStorage = refreshTokenStorage;
    }

    public async Task<TokenResponse> Handle(LoginUserRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.ToLowerInvariant();

        var user = await _dbContext.Users
            .Include(user => user.Roles)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user is null)
            throw new SecurityException("Invalid credentials");

        if (!await _passwordHashAlgorithm.ValidateAsync(request.Password, user.PasswordHash))
            throw new SecurityException("Invalid credentials");

        return new TokenResponse
        {
            AccessToken = _accessTokenProvider.Issue(user),
            RefreshToken = _refreshTokenStorage.Issue(user.Id, request.RememberMe)
        };
    }
}