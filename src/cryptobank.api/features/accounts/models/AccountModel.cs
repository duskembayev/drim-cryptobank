using cryptobank.api.features.accounts.domain;

namespace cryptobank.api.features.accounts.models;

public record AccountModel(string Id, Currency Currency, decimal Balance);