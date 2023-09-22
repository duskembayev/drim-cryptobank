using cryptobank.api.db;
using cryptobank.api.features.accounts.domain;
using cryptobank.api.features.accounts.endpoints.transfer;
using cryptobank.api.features.users.domain;
using cryptobank.api.features.users.services;
using cryptobank.api.tests.extensions;

namespace cryptobank.api.tests.features.accounts.endpoints;

public class TransferTests : IClassFixture<ApplicationFixture>, IAsyncLifetime
{
    private readonly ApplicationFixture _fixture;
    private User? _user1;
    private User? _user2;

    public TransferTests(ApplicationFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        using var scope = _fixture.AppFactory.Services.CreateScope();
        var hashAlgorithm = scope.ServiceProvider.GetRequiredService<IPasswordHashAlgorithm>();

        _user1 = await CreateUserAsync("user1@example.com",
            new (Currency currency, decimal balance)[]
            {
                (Currency.EUR, 1200),
                (Currency.USD, 200)
            }, hashAlgorithm);

        _user2 = await CreateUserAsync("user2@example.com",
            new (Currency currency, decimal balance)[]
            {
                (Currency.KZT, 100000),
                (Currency.USD, 800)
            }, hashAlgorithm);

        var dbContext = scope.ServiceProvider.GetRequiredService<CryptoBankDbContext>();
        dbContext.Attach(Role.Detached.User);
        dbContext.Users.AddRange(_user1, _user2);
        await dbContext.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        using var scope = _fixture.AppFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CryptoBankDbContext>();

        dbContext.RemoveRange(_user1!, _user2!);
        await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task ShouldTransferMoney()
    {
        const int amount = 100;
        const string comment = "Have a great day!";

        var client = _fixture.CreateClient(_user1);
        var sourceAccount = _user1!.Accounts.Single(account => account.Currency == Currency.USD);
        var targetAccount = _user2!.Accounts.Single(account => account.Currency == Currency.USD);

        var result = await client.POSTAsync<TransferRequest, TransferModel>("/accounts/transfer",
            new TransferRequest
            {
                SourceAccountId = sourceAccount.AccountId,
                TargetAccountId = targetAccount.AccountId,
                Amount = amount,
                Comment = comment
            });

        result.ShouldBeOk();
        result.Result.ShouldNotBeNull();
        result.Result.Id.ShouldBeGreaterThan(0);

        using var scope = _fixture.AppFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CryptoBankDbContext>();

        dbContext.Accounts
            .Single(a => a.AccountId == sourceAccount.AccountId)
            .Balance.ShouldBe(amount);

        dbContext.Accounts
            .Single(a => a.AccountId == targetAccount.AccountId)
            .Balance.ShouldBe(900);

        var transfer = dbContext.InternalTransfers.Single(t => t.Id == result.Result.Id);

        transfer.SourceUserId.ShouldBe(_user1.Id);
        transfer.SourceAccountId.ShouldBe(sourceAccount.AccountId);
        transfer.SourceCurrency.ShouldBe(sourceAccount.Currency);
        transfer.SourceAmount.ShouldBe(amount);
        transfer.TargetUserId.ShouldBe(_user2.Id);
        transfer.TargetAccountId.ShouldBe(targetAccount.AccountId);
        transfer.TargetCurrency.ShouldBe(targetAccount.Currency);
        transfer.TargetAmount.ShouldBe(amount);
        transfer.Comment.ShouldBe(comment);
        transfer.ConversionRate.ShouldBe(1);
        transfer.DateOfCreation.ShouldBe(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public async Task ShouldConvertAmountOnTransfer()
    {
        var client = _fixture.CreateClient(_user1);
        var sourceAccount = _user1!.Accounts.Single(account => account.Currency == Currency.USD);
        var targetAccount = _user2!.Accounts.Single(account => account.Currency == Currency.KZT);

        var result = await client.POSTAsync<TransferRequest, TransferModel>("/accounts/transfer",
            new TransferRequest
            {
                SourceAccountId = sourceAccount.AccountId,
                TargetAccountId = targetAccount.AccountId,
                Amount = 200
            });

        result.ShouldBeOk();
        result.Result.ShouldNotBeNull();
        result.Result.Id.ShouldBeGreaterThan(0);

        using var scope = _fixture.AppFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CryptoBankDbContext>();

        dbContext.Accounts
            .Single(a => a.AccountId == sourceAccount.AccountId)
            .Balance.ShouldBe(0);

        dbContext.Accounts
            .Single(a => a.AccountId == targetAccount.AccountId)
            .Balance.ShouldBe(100000 + 94360.2m, 0.1m);

        var transfer = dbContext.InternalTransfers.Single(t => t.Id == result.Result.Id);

        transfer.SourceUserId.ShouldBe(_user1.Id);
        transfer.SourceAccountId.ShouldBe(sourceAccount.AccountId);
        transfer.SourceCurrency.ShouldBe(sourceAccount.Currency);
        transfer.SourceAmount.ShouldBe(200);
        transfer.TargetUserId.ShouldBe(_user2.Id);
        transfer.TargetAccountId.ShouldBe(targetAccount.AccountId);
        transfer.TargetCurrency.ShouldBe(targetAccount.Currency);
        transfer.TargetAmount.ShouldBe(94360.2m, 0.1m);
        transfer.ConversionRate.ShouldBe(471.8m, 0.1m);
        transfer.DateOfCreation.ShouldBe(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public async Task ShouldReturnErrorWhenInsufficientFunds()
    {
        var client = _fixture.CreateClient(_user1);
        var sourceAccount = _user1!.Accounts.Single(account => account.Currency == Currency.USD);
        var targetAccount = _user2!.Accounts.Single(account => account.Currency == Currency.KZT);

        var result = await client.POSTAsync<TransferRequest, ProblemDetails>("/accounts/transfer",
            new TransferRequest
            {
                SourceAccountId = sourceAccount.AccountId,
                TargetAccountId = targetAccount.AccountId,
                Amount = 500
            });

        result.ShouldBeProblem(HttpStatusCode.Conflict, "accounts:transfer:insufficient_funds");
    }

    [Fact]
    public async Task ShouldReturnErrorWhenTargetNotFound()
    {
        var client = _fixture.CreateClient(_user1);
        var sourceAccount = _user1!.Accounts.Single(account => account.Currency == Currency.USD);

        var result = await client.POSTAsync<TransferRequest, ProblemDetails>("/accounts/transfer",
            new TransferRequest
            {
                SourceAccountId = sourceAccount.AccountId,
                TargetAccountId = "some_invalid_account_id",
                Amount = 500
            });

        result.ShouldBeProblem(HttpStatusCode.Conflict, "accounts:transfer:target_account_not_found");
    }

    [Fact]
    public async Task ShouldReturnErrorWhenSourceNotFound()
    {
        var client = _fixture.CreateClient(_user1);
        var targetAccount = _user2!.Accounts.Single(account => account.Currency == Currency.KZT);

        var result = await client.POSTAsync<TransferRequest, ProblemDetails>("/accounts/transfer",
            new TransferRequest
            {
                SourceAccountId = "some_invalid_account_id",
                TargetAccountId = targetAccount.AccountId,
                Amount = 500
            });

        result.ShouldBeProblem(HttpStatusCode.Conflict, "accounts:transfer:source_account_not_found");
    }

    private static async Task<User> CreateUserAsync(
        string email,
        IReadOnlyList<(Currency currency, decimal balance)> amountOfAccounts,
        IPasswordHashAlgorithm hashAlgorithm)
    {
        var user = new User
        {
            Email = email,
            PasswordHash = await hashAlgorithm.HashAsync("P@s$w0rd"),
            Roles = {Role.Detached.User},
            DateOfBirth = new DateOnly(2000, 1, 1),
            DateOfRegistration = new DateTime(2019, 1, 1, 1, 1, 1, DateTimeKind.Utc)
        };

        for (var i = 0; i < amountOfAccounts.Count; i++)
        {
            var account = new Account
            {
                AccountId = $"{email}+{i}",
                DateOfOpening = new DateTime(2019, 1, 1, 1, 1, 1, DateTimeKind.Utc),
                Currency = amountOfAccounts[i].currency,
                Balance = amountOfAccounts[i].balance
            };

            user.Accounts.Add(account);
        }

        return user;
    }
}