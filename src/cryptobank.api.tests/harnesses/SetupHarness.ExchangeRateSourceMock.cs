using System.Collections.Immutable;
using cryptobank.api.features.accounts.domain;
using cryptobank.api.utils.exchange;

namespace cryptobank.api.tests.harnesses;

internal partial class SetupHarness
{
    private class ExchangeRateSourceMock : IExchangeRateSource
    {
        private readonly ImmutableDictionary<Currency, decimal> _rates;

        public ExchangeRateSourceMock()
        {
            var rates = ImmutableDictionary.CreateBuilder<Currency, decimal>();

            rates[Currency.BTC] = 0.000039610816m;
            rates[Currency.CHF] = 0.959235m;
            rates[Currency.CNY] = 7.796933m;
            rates[Currency.EUR] = 1m;
            rates[Currency.GBP] = 0.865219m;
            rates[Currency.KZT] = 503.979919m;
            rates[Currency.RUB] = 103.121772m;
            rates[Currency.USD] = 1.068204m;

            _rates = rates.ToImmutable();
        }
    
        public Task<ImmutableDictionary<Currency, decimal>> GetRatesAsync()
        {
            return Task.FromResult(_rates);
        }
    }
}