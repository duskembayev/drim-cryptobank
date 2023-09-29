namespace cryptobank.api.utils.exchange;

public static class SetupExtensions
{
    public static IServiceCollection AddExchangeRates(this IServiceCollection @this)
    {
        @this
            .AddOptions<ExchangeRateOptions>()
            .BindConfiguration("ExchangeRate");

        @this
            .AddHttpClient("fixer", (s, c) =>
            {
                var options = s.GetRequiredService<IOptions<ExchangeRateOptions>>();
                c.BaseAddress = new Uri(options.Value.BaseUrl);
            });

        return @this;
    }
}