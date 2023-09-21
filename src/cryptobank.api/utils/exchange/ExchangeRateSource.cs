using System.Text.Json;
using cryptobank.api.features.accounts.domain;
using cryptobank.api.redis;

namespace cryptobank.api.utils.exchange;

[Singleton<IExchangeRateSource>]
public class ExchangeRateSource : IExchangeRateSource
{
    private const string CurrencyRatesRedisKey = "currency:rates";

    private readonly IRedisConnection _redisConnection;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<ExchangeRateOptions> _options;
    private readonly ILogger<ExchangeRateSource> _logger;

    public ExchangeRateSource(
        IRedisConnection redisConnection,
        IHttpClientFactory httpClientFactory,
        IOptions<ExchangeRateOptions> options,
        ILogger<ExchangeRateSource> logger)
    {
        _redisConnection = redisConnection;
        _httpClientFactory = httpClientFactory;
        _options = options;
        _logger = logger;
    }

    public async Task<ImmutableDictionary<Currency, decimal>> GetRatesAsync()
    {
        var rates = await GetCachedRatesAsync();

        if (rates != null)
            return rates;

        rates = await LoadRatesAsync();
        await SetCachedRatesAsync(rates);
        return rates;
    }

    private async ValueTask<ImmutableDictionary<Currency, decimal>> LoadRatesAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("fixer");
        using var response = await httpClient.GetAsync("latest?access_key=" + _options.Value.ApiKey);

        if (!response.IsSuccessStatusCode)
            throw new LogicException("exchange_rate:load_error", "Failed to load exchange rates");

        var latestData = await response.Content.ReadFromJsonAsync<FixerLatestData>();

        if (latestData is not { Success: true })
            throw new LogicException("exchange_rate:load_error", "Failed to load exchange rates");

        var ratesBuilder = ImmutableDictionary.CreateBuilder<Currency, decimal>();

        foreach (var (currency, rate) in latestData.Rates)
        {
            if (!Enum.TryParse<Currency>(currency, out var typedCurrency))
                continue;

            ratesBuilder[typedCurrency] = rate;
        }

        var missedCurrencies = Enum.GetValues<Currency>()
            .Where(c => !ratesBuilder.ContainsKey(c))
            .ToArray();

        if (missedCurrencies.Length > 0)
            _logger.LogError(
                "One or more currencies are not supported by rate provider: {currencies}",
                string.Join(", ", missedCurrencies));

        return ratesBuilder.ToImmutable();
    }

    private async ValueTask<ImmutableDictionary<Currency, decimal>?> GetCachedRatesAsync()
    {
        var ratesJson = await _redisConnection.Database.StringGetAsync(CurrencyRatesRedisKey);

        if (ratesJson is { IsNullOrEmpty: true })
            return null;

        try
        {
            return JsonSerializer.Deserialize<ImmutableDictionary<Currency, decimal>>(ratesJson.ToString());
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private async ValueTask SetCachedRatesAsync(ImmutableDictionary<Currency, decimal> rates)
    {
        var ratesJson = JsonSerializer.Serialize(rates);
        await _redisConnection.Database.StringSetAsync(CurrencyRatesRedisKey, ratesJson, _options.Value.Ttl);
    }

    private record FixerLatestData(
        bool Success, long Timestamp, string Base, DateOnly Date,
        ImmutableDictionary<string, decimal> Rates);
}