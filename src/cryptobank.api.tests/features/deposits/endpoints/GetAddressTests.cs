﻿using cryptobank.api.db;
using cryptobank.api.features.accounts.domain;
using cryptobank.api.features.deposits.domain;
using cryptobank.api.features.deposits.endpoints.getAddress;
using cryptobank.api.tests.extensions;

namespace cryptobank.api.tests.features.deposits.endpoints;

[Collection(DepositsCollection.Name)]
public class GetAddressTests
{
    private const string Xpub =
        "tpubD6NzVbkrYhZ4YAoLbobYyBFnWrLokvou8GC8jimnijV6ymg9uMxNM5ioov7eZkSDjqDaN81UDaz9y9G2CNfohhGMaesbCY22ziXZzZzEKuK";

    private readonly HttpClient _client;
    private readonly ApplicationFixture _fixture;

    public GetAddressTests(ApplicationFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.HttpClient.CreateClient(_fixture.Setup.User);
    }

    [Fact]
    public async Task ShouldGenerateAddress()
    {
        const uint derivationIndex = 10u;
        const string expectedAddress = "tb1qwt9vjywsjnlv88xw0qpjxys45cf4r50h4m2nky";

        var (accountId, xpubId) = await _fixture.Database.ExecuteAsync(async db =>
        {
            var account = await CreateAccountAsync(db, "my-btc-account", Currency.BTC);
            var xpub = await SetupXpubAsync(db, derivationIndex);
            return (account.AccountId, xpub.Id);
        });

        var result = await _client.GETAsync<GetAddressRequest, DepositAddressModel>(
            "/deposits/address",
            new GetAddressRequest {AccountId = accountId});

        var address = await _fixture.Database
            .ExecuteAsync(db => db.DepositAddresses.SingleOrDefaultAsync(d => d.AccountId == accountId));

        result.ShouldBeOk();
        result.Result.ShouldNotBeNull();
        result.Result.Address.ShouldBe(expectedAddress);

        address.ShouldNotBeNull();
        address.DerivationIndex.ShouldBe(derivationIndex);
        address.CryptoAddress.ShouldBe(expectedAddress);
        address.XpubId.ShouldBe(xpubId);

        var xpub = await _fixture.Database
            .ExecuteAsync(db => db.Xpubs.SingleAsync(x => x.Id == xpubId));

        xpub.NextDerivationIndex.ShouldBe(derivationIndex + 1);
    }

    [Fact]
    public async Task ShouldReturnAlreadyExistingAddress()
    {
        const string expectedAddress = "some-generated-before-address";

        var accountId = await _fixture.Database.ExecuteAsync(async db =>
        {
            var account = await CreateAccountAsync(db, "my-account", Currency.EUR);
            var xpub = await SetupXpubAsync(db, 12);

            db.DepositAddresses.Add(new DepositAddress
            {
                AccountId = account.AccountId,
                XpubId = xpub.Id,
                DerivationIndex = 10,
                CryptoAddress = expectedAddress
            });

            await db.SaveChangesAsync();
            return account.AccountId;
        });

        var result = await _client.GETAsync<GetAddressRequest, DepositAddressModel>(
            "/deposits/address",
            new GetAddressRequest {AccountId = accountId});

        result.Result?.Address.ShouldBe(expectedAddress);
    }

    [Fact]
    public async Task ShouldReturnErrorWhenAccountNotFound()
    {
        var result = await _client.GETAsync<GetAddressRequest, ProblemDetails>(
            "/deposits/address",
            new GetAddressRequest {AccountId = "some-unknown-account"});

        result.ShouldBeValidationProblem(nameof(GetAddressRequest.AccountId), "deposits:get_address:account_not_found");
    }

    [Fact]
    public async Task ShouldReturnErrorWhenAccountNotBtc()
    {
        var account = await _fixture.Database
            .ExecuteAsync(async db => await CreateAccountAsync(db, "my-eur-account", Currency.EUR));

        var result = await _client.GETAsync<GetAddressRequest, ProblemDetails>(
            "/deposits/address",
            new GetAddressRequest {AccountId = account.AccountId});

        result.ShouldBeValidationProblem(nameof(GetAddressRequest.AccountId), "deposits:get_address:account_not_btc");
    }

    private async ValueTask<Account> CreateAccountAsync(
        CryptoBankDbContext dbContext, string accountId, Currency currency)
    {
        var account = new Account
        {
            AccountId = accountId,
            User = _fixture.Setup.User,
            Currency = currency,
            Balance = 0,
            DateOfOpening = new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        dbContext.Attach(_fixture.Setup.User);
        dbContext.Accounts.Add(account);
        await dbContext.SaveChangesAsync();
        return account;
    }

    private async ValueTask<Xpub> SetupXpubAsync(CryptoBankDbContext dbContext, uint nextDerivationIndex)
    {
        var xpub = new Xpub
        {
            Value = Xpub,
            Currency = Currency.BTC,
            NextDerivationIndex = nextDerivationIndex
        };

        dbContext.Xpubs.Add(xpub);
        await dbContext.SaveChangesAsync();
        return xpub;
    }
}