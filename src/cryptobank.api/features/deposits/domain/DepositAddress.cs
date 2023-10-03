namespace cryptobank.api.features.deposits.domain;

public class DepositAddress
{
    public required string AccountId { get; init; }
    public required int XpubId { get; init; }
    public required uint DerivationIndex { get; init; }
    public required string CryptoAddress { get; init; }
}