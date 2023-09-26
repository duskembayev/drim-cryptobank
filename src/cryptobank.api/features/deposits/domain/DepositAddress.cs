namespace cryptobank.api.features.deposits.domain;

public class DepositAddress
{
    public required int UserId { get; init; }
    public required int XpubId { get; init; }
    public required int DerivationIndex { get; init; }
    public required string CryptoAddress { get; init; }
}