using cryptobank.api.features.accounts.domain;

namespace cryptobank.api.features.deposits.services;

public interface ICryptoAddressGenerator
{
    Task<(int XpubId, uint derivationIndex, string address)> GenerateAsync(Currency currency, CancellationToken cancellationToken);
}