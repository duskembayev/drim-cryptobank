using cryptobank.api.features.news.config;

namespace cryptobank.api.features.users;

public static class SetupExtensions
{
    private const string NewsSectionKey = "News";
    
    public static WebApplicationBuilder AddUsers(this WebApplicationBuilder @this)
    {
        @this.Services
            .Configure<NewsOptions>(@this.Configuration.GetSection(NewsSectionKey));

        return @this;
    }
}