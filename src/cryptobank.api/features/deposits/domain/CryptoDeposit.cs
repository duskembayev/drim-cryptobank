using cryptobank.api.features.accounts.domain;

namespace cryptobank.api.features.deposits.domain;

public class CryptoDeposit
{
    public long Id { get; init; }
    public string AccountId { get; init; } = string.Empty;
    public string TxId { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DepositStatus Status { get; init; }
    public DateTime DateOfCreation { get; init; }
    public DateTime? DateOfCompletion { get; init; }
    public DepositAddress? Address { get; init; }
    public Account? Account { get; init; }
}