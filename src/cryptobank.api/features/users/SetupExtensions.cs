using cryptobank.api.features.users.config;
using Microsoft.IdentityModel.JsonWebTokens;

namespace cryptobank.api.features.users;

public static class SetupExtensions
{
    private const string RegisterUserSectionKey = "RegisterUser";
    private const string AccessTokenSectionKey = "AccessToken";

    public static WebApplicationBuilder AddUsers(this WebApplicationBuilder @this)
    {
        @this.Services
            .Configure<RegisterUserOptions>(@this.Configuration.GetSection(RegisterUserSectionKey))
            .Configure<AccessTokenOptions>(@this.Configuration.GetSection(AccessTokenSectionKey))
            .AddSingleton<JsonWebTokenHandler>();

        return @this;
    }
}