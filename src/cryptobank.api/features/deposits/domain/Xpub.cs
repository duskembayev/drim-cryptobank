using cryptobank.api.features.accounts.domain;

namespace cryptobank.api.features.deposits.domain;

public class Xpub
{
    public int Id { get; init; }
    public required Currency Currency { get; init; }
    public required string Value { get; init; }
    public int NextDerivationIndex { get; set; } = 0;
}