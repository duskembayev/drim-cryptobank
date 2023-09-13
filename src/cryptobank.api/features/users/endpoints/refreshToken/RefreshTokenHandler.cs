using cryptobank.api.features.users.models;
using cryptobank.api.features.users.services;

namespace cryptobank.api.features.users.endpoints.refreshToken;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenRequest, TokenModel>
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

    public async Task<TokenModel> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var nextRefreshToken = _refreshTokenStorage.Renew(request.RefreshToken);

        if (nextRefreshToken is null)
            throw new SecurityException(
                "users:refresh_token:expired",
                "Token expired or revoked");

        var user = await _dbContext.Users.SingleAsync(
            user => user.Id == nextRefreshToken.Value.UserId,
            cancellationToken);

        return new TokenModel
        {
            AccessToken = _accessTokenProvider.Issue(user),
            RefreshToken = nextRefreshToken.Value.Token
        };
    }
}