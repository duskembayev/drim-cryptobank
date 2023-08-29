using System.Security;
using cryptobank.api.features.users.requests;
using cryptobank.api.features.users.responses;
using cryptobank.api.features.users.services;

namespace cryptobank.api.features.users.handlers;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenRequest, TokenResponse>
{
    private readonly IAccessTokenProvider _accessTokenProvider;
    private readonly CryptoBankDbContext _dbContext;
    private readonly IRefreshTokenStorage _refreshTokenStorage;

    public RefreshTokenHandler(
        CryptoBankDbContext dbContext,
        IAccessTokenProvider accessTokenProvider,
        IRefreshTokenStorage refreshTokenStorage)
    {
        _dbContext = dbContext;
        _accessTokenProvider = accessTokenProvider;
        _refreshTokenStorage = refreshTokenStorage;
    }

    public async Task<TokenResponse> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var nextRefreshToken = _refreshTokenStorage.Renew(request.RefreshToken);

        if (nextRefreshToken is null)
            throw new SecurityException("Token expired or revoked");

        var user = await _dbContext.Users.SingleAsync(
            user => user.Id == nextRefreshToken.Value.UserId,
            cancellationToken);

        return new TokenResponse
        {
            AccessToken = _accessTokenProvider.Issue(user),
            RefreshToken = nextRefreshToken.Value.Token
        };
    }
}