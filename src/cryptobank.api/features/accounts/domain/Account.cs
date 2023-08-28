using cryptobank.api.features.users.domain;

namespace cryptobank.api.features.accounts.domain;

public class Account
{
    public string AccountId { get; init; } = string.Empty;
    public int UserId { get; init; }
    public User User { get; init; } = User.Empty;
    public Currency Currency { get; init; } = Currency.USD;
    public decimal Balance { get; init; }
    public DateTime DateOfOpening { get; init; } = DateTime.UnixEpoch;
}