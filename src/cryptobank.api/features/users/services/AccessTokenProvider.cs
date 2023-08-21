using System.Security.Claims;
using System.Text;
using cryptobank.api.features.users.config;
using cryptobank.api.features.users.domain;
using cryptobank.api.utils.environment;
using Enhanced.DependencyInjection;
using Microsoft.Extensions.Options;
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

    public AccessTokenProvider(
        JsonWebTokenHandler jsonWebTokenHandler,
        ITimeProvider timeProvider,
        IOptions<AccessTokenOptions> options)
    {
        _jsonWebTokenHandler = jsonWebTokenHandler;
        _timeProvider = timeProvider;
        _options = options;
    }

    public Task<string> IssueAsync(User user, CancellationToken cancellationToken)
    {
        var utcNow = _timeProvider.UtcNow;
        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Value.Issuer,
            Audience = _options.Value.Audience,
            Subject = new ClaimsIdentity(new List<Claim>
            {
                new(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new("roles", user.Roles.Aggregate(Roles.None, (roles, role) => roles | (Roles) role.Id).ToString("D")),
            }, "Bearer"),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.SecurityKey)),
                SecurityAlgorithm),
            Expires = utcNow.Add(_options.Value.Expiration),
            IssuedAt = utcNow,
            NotBefore = utcNow,
        };

        var token = _jsonWebTokenHandler.CreateToken(securityTokenDescriptor);
        return Task.FromResult(token);
    }
}