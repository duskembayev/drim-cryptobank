using cryptobank.api.db;
using cryptobank.api.features.accounts.domain;
using cryptobank.api.features.accounts.endpoints.reportOpenedDaily;
using cryptobank.api.features.users.domain;
using cryptobank.api.features.users.services;
using cryptobank.api.tests.extensions;

namespace cryptobank.api.tests.features.accounts.endpoints;

public class ReportOpenedDailyTests : IClassFixture<ApplicationFixture>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly ApplicationFixture _fixture;
    private readonly List<User> _users = new();

    public ReportOpenedDailyTests(ApplicationFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.CreateClient(fixture.Analyst);
    }

    public async Task InitializeAsync()
    {
        using var scope = _fixture.AppFactory.Services.CreateScope();
        var hashAlgorithm = scope.ServiceProvider.GetRequiredService<IPasswordHashAlgorithm>();

        _users.Add(await CreateUserAsync("user1@example.com", new[]
        {
            new DateOnly(2020, 1, 4),
            new DateOnly(2020, 1, 7)
        }, hashAlgorithm));

        _users.Add(await CreateUserAsync("user2@example.com", new[]
        {
            new DateOnly(2020, 1, 1),
            new DateOnly(2020, 1, 5)
        }, hashAlgorithm));

        _users.Add(await CreateUserAsync("user3@example.com", new[]
        {
            new DateOnly(2020, 1, 1),
            new DateOnly(2020, 1, 2)
        }, hashAlgorithm));

        var dbContext = scope.ServiceProvider.GetRequiredService<CryptoBankDbContext>();
        dbContext.Attach(Role.Detached.User);
        dbContext.Users.AddRange(_users);
        await dbContext.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        using var scope = _fixture.AppFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CryptoBankDbContext>();

        dbContext.RemoveRange(_users);
        await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task ShouldReturnReport()
    {
        var result = await _client.GETAsync<ReportOpenedDailyRequest, IReadOnlyCollection<OpenedDailyModel>>(
            "/accounts/reports/openedDaily", new ReportOpenedDailyRequest
            {
                Start = new DateOnly(2020, 1, 1),
                End = new DateOnly(2020, 1, 5)
            });

        result.ShouldBeOk();
        result.Result.ShouldNotBeNull();
        result.Result.ShouldBe(new[]
        {
            new OpenedDailyModel(new DateOnly(2020, 1, 1), 2),
            new OpenedDailyModel(new DateOnly(2020, 1, 2), 1),
            new OpenedDailyModel(new DateOnly(2020, 1, 4), 1),
            new OpenedDailyModel(new DateOnly(2020, 1, 5), 1)
        });
    }

    [Fact]
    public async Task ShouldNotReturnReportWhenUserIsAdministrator()
    {
        using var adminClient = _fixture.CreateClient(_fixture.Administrator);

        var result = await adminClient.GETAsync<ReportOpenedDailyRequest, IReadOnlyCollection<OpenedDailyModel>>(
            "/accounts/reports/openedDaily", new ReportOpenedDailyRequest
            {
                Start = new DateOnly(2020, 1, 1),
                End = new DateOnly(2020, 1, 5)
            });

        result.ShouldBeWithStatus(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ShouldNotReturnReportWhenUserIsUser()
    {
        using var userClient = _fixture.CreateClient(_fixture.User);

        var result = await userClient.GETAsync<ReportOpenedDailyRequest, IReadOnlyCollection<OpenedDailyModel>>(
            "/accounts/reports/openedDaily", new ReportOpenedDailyRequest
            {
                Start = new DateOnly(2020, 1, 1),
                End = new DateOnly(2020, 1, 5)
            });

        result.ShouldBeWithStatus(HttpStatusCode.Forbidden);
    }

    private static async Task<User> CreateUserAsync(
        string email,
        IReadOnlyList<DateOnly> openDateOfAccounts,
        IPasswordHashAlgorithm hashAlgorithm)
    {
        var user = new User
        {
            Email = email,
            PasswordHash = await hashAlgorithm.HashAsync("P@s$w0rd"),
            Roles = { Role.Detached.User },
            DateOfBirth = new DateOnly(2000, 1, 1),
            DateOfRegistration = new DateTime(2019, 1, 1, 1, 1, 1, DateTimeKind.Utc)
        };

        for (var i = 0; i < openDateOfAccounts.Count; i++)
        {
            var account = new Account
            {
                AccountId = $"{email}+{i}",
                DateOfOpening = openDateOfAccounts[i].ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)
            };

            user.Accounts.Add(account);
        }

        return user;
    }
}