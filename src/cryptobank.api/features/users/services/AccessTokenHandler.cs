using System.Security.Claims;
using System.Text.Encodings.Web;
using cryptobank.api.features.users.config;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace cryptobank.api.features.users.services;

public class AccessTokenHandler : AuthenticationHandler<AccessTokenOptions>
{
    private readonly JsonWebTokenHandler _jsonWebTokenHandler;

    public AccessTokenHandler(
        JsonWebTokenHandler jsonWebTokenHandler,
        IOptionsMonitor<AccessTokenOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
        _jsonWebTokenHandler = jsonWebTokenHandler;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Request.Headers.Authorization.Count != 1)
            return AuthenticateResult.Fail("Invalid \"Authorization\" headers");

        var headerParts = Request.Headers.Authorization[0]?.Split(' ');

        if (headerParts is not [AccessTokenConstants.Bearer, var token])
            return AuthenticateResult.Fail("Invalid \"Authorization\" headers");

        var validationParameters = GetTokenValidationParameters();
        var validationResult = await _jsonWebTokenHandler.ValidateTokenAsync(token, validationParameters);

        if (!validationResult.IsValid)
            return AuthenticateResult.Fail("Invalid access token");

        var claimsPrincipal = new ClaimsPrincipal(validationResult.ClaimsIdentity);
        return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
    }

    private TokenValidationParameters GetTokenValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = Options.Audience,
            ValidIssuer = Options.ClaimsIssuer,
            IssuerSigningKey = Options.GetSecurityKey(),
            RequireAudience = true,
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            AuthenticationType = AccessTokenConstants.Bearer,
            RoleClaimType = AccessTokenConstants.ClaimsTypes.Role,
            NameClaimType = AccessTokenConstants.ClaimsTypes.Email
        };
    }
}