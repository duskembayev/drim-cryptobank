using cryptobank.api.features.accounts.config;

namespace cryptobank.api.features.accounts;

public static class SetupExtensions
{
    private const string AccountsSectionKey = "Features:Accounts";

    public static WebApplicationBuilder AddAccounts(this WebApplicationBuilder @this)
    {
        @this.Services
            .Configure<AccountsOptions>(@this.Configuration.GetSection(AccountsSectionKey));

        return @this;
    }
}