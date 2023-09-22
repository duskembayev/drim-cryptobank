namespace cryptobank.api.utils.exchange;

public static class SetupExtensions
{
    public static WebApplicationBuilder AddExchangeRates(this WebApplicationBuilder @this)
    {
        @this.Services
            .AddOptions<ExchangeRateOptions>()
            .BindConfiguration("ExchangeRate");

        @this.Services
            .AddHttpClient("fixer", (s, c) =>
            {
                var options = s.GetRequiredService<IOptions<ExchangeRateOptions>>();
                c.BaseAddress = new Uri(options.Value.BaseUrl);
            });

        return @this;
    }
}