namespace cryptobank.api.features.accounts;

public static class SetupExtensions
{
    public static WebApplicationBuilder AddAccounts(this WebApplicationBuilder @this)
    {
        @this.Services.AddHttpClient("fixer", c => { c.BaseAddress = new Uri("http://data.fixer.io/api"); });

        return @this;
    }
}