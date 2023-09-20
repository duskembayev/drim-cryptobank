using System.Collections.Immutable;
using cryptobank.api.features.accounts.domain;
using cryptobank.api.features.accounts.services;
using cryptobank.api.utils.exchange;

namespace cryptobank.api.tests.features.accounts.services;

public class CurrencyConverterTests
{
    private readonly CurrencyConverter _currencyConverter;

    public CurrencyConverterTests()
    {
        var exchangeRateSource = Substitute.For<IExchangeRateSource>();
        var rates = ImmutableDictionary.CreateBuilder<Currency, decimal>();

        rates[Currency.BTC] = 0.000039610816m;
        rates[Currency.CHF] = 0.959235m;
        rates[Currency.CNY] = 7.796933m;
        rates[Currency.EUR] = 1m;
        rates[Currency.GBP] = 0.865219m;
        rates[Currency.KZT] = 503.979919m;
        rates[Currency.RUB] = 103.121772m;
        rates[Currency.USD] = 1.068204m;

        exchangeRateSource.GetRatesAsync()
            .Returns(Task.FromResult(rates.ToImmutable()));

        _currencyConverter = new CurrencyConverter(exchangeRateSource);
    }

    [Fact]
    public async Task ShouldConvert()
    {
        var (amount, rate) = await _currencyConverter
            .ConvertAsync(Currency.EUR, Currency.KZT, 2);

        rate.ShouldBe(503.98m, 0.01m);
        amount.ShouldBe(1007.96m, 0.01m);
    }

    [Fact]
    public async Task ShouldConvertBack()
    {
        var (amount, rate) = await _currencyConverter
            .ConvertAsync(Currency.KZT, Currency.USD, 946m);

        rate.ShouldBe(0.00212m, 0.0001m);
        amount.ShouldBe(2, 0.01m);
    }

    [Fact]
    public async Task ShouldConvertWhenSameCurrencies()
    {
        var (amount, rate) = await _currencyConverter
            .ConvertAsync(Currency.CHF, Currency.CHF, 157);

        rate.ShouldBe(1, 0.01m);
        amount.ShouldBe(157);
    }

    [Theory]
    [InlineData(Currency.JPY, Currency.GBP)]
    [InlineData(Currency.GBP, Currency.JPY)]
    public async Task ShouldThrowExceptionWhenCurrencyNotDefined(Currency source, Currency target)
    {
        var exception = await Assert.ThrowsAsync<ApplicationException>(
            () => _currencyConverter.ConvertAsync(source, target, 10m));

        exception.Message.ShouldBe("Unknown currency");
    }
}