using System.Security.Claims;
using cryptobank.api.features.users.config;
using cryptobank.api.features.users.domain;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace cryptobank.api.features.users.services;

[Singleton<IAccessTokenProvider>]
public class AccessTokenProvider : IAccessTokenProvider
{
    private const string SecurityAlgorithm = SecurityAlgorithms.HmacSha256;

    private readonly JsonWebTokenHandler _jsonWebTokenHandler;
    private readonly IOptions<AccessTokenOptions> _options;
    private readonly SecurityKey _securityKey;
    private readonly ITimeProvider _timeProvider;

    public AccessTokenProvider(
        JsonWebTokenHandler jsonWebTokenHandler,
        ITimeProvider timeProvider,
        IOptions<AccessTokenOptions> options)
    {
        _jsonWebTokenHandler = jsonWebTokenHandler;
        _timeProvider = timeProvider;
        _options = options;

        _securityKey = _options.Value.GetSecurityKey();
    }

    public string Issue(User user)
    {
        var utcNow = _timeProvider.UtcNow;

        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Value.ClaimsIssuer,
            Audience = _options.Value.Audience,
            Expires = utcNow.Add(_options.Value.Expiration),
            IssuedAt = utcNow,
            NotBefore = utcNow,
            Subject = GetClaims(user),
            SigningCredentials = new SigningCredentials(_securityKey, SecurityAlgorithm)
        };

        return _jsonWebTokenHandler.CreateToken(securityTokenDescriptor);
    }

    private static ClaimsIdentity GetClaims(User user)
    {
        var claims = new ClaimsIdentity(
            AccessTokenConstants.Bearer,
            AccessTokenConstants.ClaimsTypes.Email,
            AccessTokenConstants.ClaimsTypes.Role);

        claims.AddClaim(new Claim(AccessTokenConstants.ClaimsTypes.Id, user.Id.ToString()));
        claims.AddClaim(new Claim(AccessTokenConstants.ClaimsTypes.Email, user.Email));

        foreach (var role in user.Roles)
            claims.AddClaim(new Claim(AccessTokenConstants.ClaimsTypes.Role, role.Name));

        return claims;
    }
}