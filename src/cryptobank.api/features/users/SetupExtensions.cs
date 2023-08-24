using cryptobank.api.features.users.config;
using Microsoft.IdentityModel.JsonWebTokens;

namespace cryptobank.api.features.users;

public static class SetupExtensions
{
    private const string RegisterUserSectionKey = "Features:Users:RegisterUser";
    private const string AccessTokenSectionKey = "Features:Users:AccessToken";
    private const string PasswordSectionKey = "Features:Users:PasswordHash";

    public static WebApplicationBuilder AddUsers(this WebApplicationBuilder @this)
    {
        @this.Services
            .Configure<RegisterUserOptions>(@this.Configuration.GetSection(RegisterUserSectionKey))
            .Configure<AccessTokenOptions>(@this.Configuration.GetSection(AccessTokenSectionKey))
            .Configure<PasswordHashOptions>(@this.Configuration.GetSection(PasswordSectionKey))
            .AddSingleton<JsonWebTokenHandler>();
    
        return @this;
    }
}