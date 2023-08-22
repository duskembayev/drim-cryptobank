using cryptobank.api.features.users.config;
using cryptobank.api.features.users.services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;

namespace cryptobank.api.features.users;

public static class SetupExtensions
{
    private const string RegisterUserSectionKey = "Features:Users:RegisterUser";
    private const string AccessTokenSectionKey = "Features:Users:AccessToken";
    private const string AuthenticationScheme = "JwtBearer";

    public static WebApplicationBuilder AddUsers(this WebApplicationBuilder @this)
    {
        @this.Services
            .Configure<RegisterUserOptions>(@this.Configuration.GetSection(RegisterUserSectionKey))
            .Configure<AccessTokenOptions>(@this.Configuration.GetSection(AccessTokenSectionKey))
            .Configure<AccessTokenOptions>(AuthenticationScheme, @this.Configuration.GetSection(AccessTokenSectionKey))
            .AddSingleton<JsonWebTokenHandler>();

        @this.Services
            .AddAuthorization(ConfigureAuthorization)
            .AddAuthentication()
            .AddScheme<AccessTokenOptions, AccessTokenHandler>(AuthenticationScheme, null);
        
        return @this;
    }

    private static void ConfigureAuthorization(AuthorizationOptions options)
    {
        var defaultPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddAuthenticationSchemes(AuthenticationScheme)
            .Build();

        options.DefaultPolicy = defaultPolicy;
    }
}