using cryptobank.api.features.accounts.domain;

namespace cryptobank.api.utils.exchange;

public interface IExchangeRateSource
{
    Task<ImmutableDictionary<Currency, decimal>> GetRatesAsync();
}