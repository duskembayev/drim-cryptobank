using cryptobank.api.features.accounts.domain;

namespace cryptobank.api.features.accounts.endpoints.listAccounts;

public record AccountModel(string Id, Currency Currency, decimal Balance);