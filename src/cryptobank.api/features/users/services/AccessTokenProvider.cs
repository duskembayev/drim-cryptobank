using System.Security.Claims;
using cryptobank.api.features.users.config;
using cryptobank.api.features.users.domain;
using cryptobank.api.utils.environment;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace cryptobank.api.features.users.services;

[ContainerEntry(ServiceLifetime.Singleton, typeof(IAccessTokenProvider))]
public class AccessTokenProvider : IAccessTokenProvider
{
    private const string SecurityAlgorithm = SecurityAlgorithms.HmacSha256;

    private readonly JsonWebTokenHandler _jsonWebTokenHandler;
    private readonly ITimeProvider _timeProvider;
    private readonly IOptions<AccessTokenOptions> _options;
    private readonly SecurityKey _securityKey;

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
            SigningCredentials = new SigningCredentials(_securityKey, SecurityAlgorithm),
        };

        return _jsonWebTokenHandler.CreateToken(securityTokenDescriptor);
    }

    private static ClaimsIdentity GetClaims(User user)
    {
        var claims = new ClaimsIdentity(AccessTokenConstants.Bearer);

        claims.AddClaim(new Claim("id", user.Id.ToString()));
        claims.AddClaim(new Claim("email", user.Email));

        foreach (var role in user.Roles)
            claims.AddClaim(new Claim("roles", role.Name));

        return claims;
    }
}