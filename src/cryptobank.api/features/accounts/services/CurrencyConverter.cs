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

    public async Task<(decimal TargetAmount, decimal Rate)> ConvertAsync(Currency source, Currency target, decimal sourceAmount)
    {
        var rate = await GetRateAsync(source, target);
        var targetAmount = sourceAmount * rate;
        return (targetAmount, rate);
    }

    private async ValueTask<decimal> GetRateAsync(Currency source, Currency target)
    {
        if (source == target)
            return 1;

        var rates = await _exchangeRateSource.GetRatesAsync();

        if (!rates.TryGetValue(source, out var sourceRate) || !rates.TryGetValue(target, out var targetRate))
            throw new ApplicationException("Unknown currency");

        return targetRate / sourceRate;
    }
}