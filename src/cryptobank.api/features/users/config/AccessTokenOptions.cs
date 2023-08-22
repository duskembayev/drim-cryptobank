using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace cryptobank.api.features.users.config;

public class AccessTokenOptions : AuthenticationSchemeOptions
{
    public AccessTokenOptions()
    {
        ClaimsIssuer = "CryptoBank";
    }

    public string Audience { get; set; } = "CryptoBank";
    public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(15);
    public string SecurityKey { get; set; } = string.Empty;

    public SecurityKey GetSecurityKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey));

    public override void Validate()
    {
        if (string.IsNullOrEmpty(ClaimsIssuer)
            || string.IsNullOrEmpty(Audience)
            || string.IsNullOrEmpty(SecurityKey)
            || Expiration < TimeSpan.FromMinutes(1))
            throw new ApplicationException("Invalid access token options");
    }
}