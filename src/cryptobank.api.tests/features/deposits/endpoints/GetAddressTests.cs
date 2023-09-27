using cryptobank.api.db;
using cryptobank.api.features.accounts.domain;
using cryptobank.api.features.deposits.endpoints.getAddress;
using cryptobank.api.tests.extensions;

namespace cryptobank.api.tests.features.deposits.endpoints;

public class GetAddressTests : IClassFixture<ApplicationFixture>, IAsyncLifetime
{
    private readonly ApplicationFixture _fixture;
    private readonly HttpClient _client;
    private Account? _account;

    public GetAddressTests(ApplicationFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.CreateClient(_fixture.User);
    }

    [Fact]
    public async Task ShouldGenerateAddress()
    {
        var result = await _client.GETAsync<GetAddressRequest, DepositAddressModel>("/deposits/address",
            new GetAddressRequest
            {
                AccountId = _account!.AccountId
            });

        result.ShouldBeOk();
        result.Result.ShouldNotBeNull();
        result.Result.Address.ShouldBe("tb1qllpcxkaaztnm938tzyar7dhghfce7n6a4cmxjw");
    }

    public async Task InitializeAsync()
    {
        using var scope = _fixture.AppFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CryptoBankDbContext>();

        _account = new Account
        {
            AccountId = "awesome-account",
            User = _fixture.User,
            Currency = Currency.BTC,
            Balance = 0,
            DateOfOpening = new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        dbContext.Attach(_fixture.User);
        dbContext.Accounts.Add(_account);
        await dbContext.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        using var scope = _fixture.AppFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CryptoBankDbContext>();
        dbContext.Accounts.Remove(_account!);
        await dbContext.SaveChangesAsync();
    }
}