using cryptobank.api.features.accounts.domain;
using cryptobank.api.features.accounts.endpoints.openAccount;
using cryptobank.api.features.users.domain;
using cryptobank.api.tests.extensions;

namespace cryptobank.api.tests.features.accounts.endpoints;

[Collection(AccountsCollection.Name)]
public class OpenAccountTests
{
    private readonly ApplicationFixture _fixture;
    private readonly HttpClient _client;

    public OpenAccountTests(ApplicationFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.HttpClient.CreateClient(_fixture.Setup.User);
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

        var account = await _fixture.Database
            .ExecuteAsync(db => db.Accounts.SingleAsync(a => a.AccountId == result.Result));

        account.UserId.ShouldBe(_fixture.Setup.User.Id);
        account.Currency.ShouldBe(currency);
        account.Balance.ShouldBe(0);
        account.DateOfOpening.ShouldBe(DateTime.UtcNow, TimeSpan.FromSeconds(3));
    }

    [Fact]
    public async Task ShouldNotOpenAccountWhenLimitReached()
    {
        var user = await _fixture.Setup.CreateUserAsync(Role.User, "mike@example.com");
        var userClient = _fixture.HttpClient.CreateClient(user);

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