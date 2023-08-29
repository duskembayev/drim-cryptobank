using cryptobank.api.features.news.config;

namespace cryptobank.api.features.news;

public static class SetupExtensions
{
    private const string NewsSectionKey = "Features:News";

    public static WebApplicationBuilder AddNews(this WebApplicationBuilder @this)
    {
        @this.Services
            .Configure<NewsOptions>(@this.Configuration.GetSection(NewsSectionKey));

        return @this;
    }
}