using cryptobank.api.features.accounts.domain;

namespace cryptobank.api.features.accounts.services;

public interface ICurrencyConverter
{
    Task<(decimal TargetAmount, decimal Rate)> ConvertAsync(Currency source, Currency target, decimal amount);
}