using cryptobank.api.features.users.config;

namespace cryptobank.api.features.news;

public static class SetupExtensions
{
    private const string RegisterUserSectionKey = "RegisterUser";
    
    public static WebApplicationBuilder AddNews(this WebApplicationBuilder @this)
    {
        @this.Services
            .Configure<RegisterUserOptions>(@this.Configuration.GetSection(RegisterUserSectionKey));

        return @this;
    }
}