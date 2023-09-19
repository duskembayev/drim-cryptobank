using cryptobank.api.features.accounts.domain;
using cryptobank.api.utils.exchange;

namespace cryptobank.api.features.accounts.services;

[Singleton<ICurrencyConverter>]
public class CurrencyConverter : ICurrencyConverter
{
    private readonly IExchangeRateSource _exchangeRateSource;

    public CurrencyConverter(IExchangeRateSource exchangeRateSource)
    {
        _exchangeRateSource = exchangeRateSource;
    }

    public async Task<decimal> ConvertAsync(Currency source, Currency target, decimal amount)
    {
        var rate = await GetRateAsync(source, target);
        return amount * rate;
    }

    private async ValueTask<decimal> GetRateAsync(Currency source, Currency target)
    {
        if (source == target)
            return 1;

        var rates = await _exchangeRateSource.GetRatesAsync();

        if (!rates.TryGetValue(source, out var sourceRate) || !rates.TryGetValue(target, out var targetRate))
            throw new LogicException("exchange_rate:unknown_currency", "Unknown currency");

        return targetRate / sourceRate;
    }
}