namespace cryptobank.api.features.accounts.config;

[Options("Accounts")]
public class AccountsOptions
{
    public int MaxAccountsPerUser { get; set; } = 5;
}