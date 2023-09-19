using cryptobank.api.db;
using cryptobank.api.features.accounts.domain;
using cryptobank.api.features.accounts.endpoints.openAccount;
using cryptobank.api.tests.extensions;

namespace cryptobank.api.tests.features.accounts.endpoints;

public class OpenAccountTests : IClassFixture<ApplicationFixture>
{
    private readonly ApplicationFixture _fixture;
    private readonly HttpClient _client;

    public OpenAccountTests(ApplicationFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.CreateClient(_fixture.User);
    }

    [Theory]
    [InlineData(Currency.EUR)]
    [InlineData(Currency.BTC)]
    public async Task ShouldOpenAccount(Currency currency)
    {
        var result = await _client.POSTAsync<OpenAccountRequest, string>("/accounts/open",
            new OpenAccountRequest
            {
                Currency = currency
            });

        result.ShouldBeOk();
        result.Result.ShouldNotBeEmpty();

        using var scope = _fixture.AppFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CryptoBankDbContext>();
        var account = await dbContext.Accounts.SingleAsync(a => a.AccountId == result.Result);

        account.UserId.ShouldBe(_fixture.User.Id);
        account.Currency.ShouldBe(currency);
        account.Balance.ShouldBe(0);
        account.DateOfOpening.ShouldBe(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task ShouldNotOpenAccountWhenLimitReached()
    {
        var user = await _fixture.AppFactory.CreateUserAsync("mike@example.com");
        var userClient = _fixture.CreateClient(user);

        var result1 = await userClient.POSTAsync<OpenAccountRequest, string>("/accounts/open",
            new OpenAccountRequest
            {
                Currency = Currency.BTC
            });

        var result2 = await userClient.POSTAsync<OpenAccountRequest, string>("/accounts/open",
            new OpenAccountRequest
            {
                Currency = Currency.EUR
            });

        var result3 = await userClient.POSTAsync<OpenAccountRequest, ProblemDetails>("/accounts/open",
            new OpenAccountRequest
            {
                Currency = Currency.GBP
            });

        result1.ShouldBeOk();
        result2.ShouldBeOk();
        result3.ShouldBeProblem(HttpStatusCode.Conflict, "accounts:create:limit_reached");
    }
}