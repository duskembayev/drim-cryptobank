using cryptobank.api.features.users.domain;
using cryptobank.api.features.users.services;
using cryptobank.api.tests.harnesses.core;
using cryptobank.api.utils.exchange;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace cryptobank.api.tests.harnesses;

internal partial class SetupHarness : Harness
{
    public const string UserEmail = "user@example.com";
    public const string UserPassword = "userP@s$w0rd";
    public const string AdministratorEmail = "admin@example.com";
    public const string AdministratorPassword = "adminP@s$w0rd";
    public const string AnalystEmail = "analyst@example.com";
    public const string AnalystPassword = "analystP@s$w0rd";

    private readonly DatabaseHarness _database;
    private User? _administrator;
    private User? _analyst;
    private User? _user;

    public SetupHarness(DatabaseHarness database)
    {
        _database = database;
    }

    internal User User
    {
        get => _user ?? throw new InvalidOperationException();
        private set => _user = value;
    }

    internal User Administrator
    {
        get => _administrator ?? throw new InvalidOperationException();
        private set => _administrator = value;
    }

    internal User Analyst
    {
        get => _analyst ?? throw new InvalidOperationException();
        private set => _analyst = value;
    }

    internal async ValueTask<User> CreateUserAsync(
        string roleName,
        string email = "harrypotter@hogwards.edu.uk",
        string password = "C@putDrac0nis")
    {
        var hashAlgorithm = Factory.Services.GetRequiredService<IPasswordHashAlgorithm>();
        var role = Role.Detached.GetByName(roleName);
        var user = new User
        {
            Email = email,
            PasswordHash = await hashAlgorithm.HashAsync(password),
            Roles = {role},
            DateOfBirth = new DateOnly(1980, 7, 31),
            DateOfRegistration = new DateTime(1999, 6, 30, 4, 31, 48, DateTimeKind.Utc)
        };

        await _database.ExecuteAsync(async context =>
        {
            context.Attach(role);
            context.Users.Add(user);
            await context.SaveChangesAsync();
        });

        return user;
    }

    protected override void OnConfigure(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Features:Accounts:MaxAccountsPerUser"] = "2",
            });
        });

        builder.ConfigureServices(collection =>
        {
            collection.AddSingleton<IExchangeRateSource, ExchangeRateSourceMock>();
        });
    }

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        _database.ThrowIfNotStarted();

        User = await CreateUserAsync(
            Role.User, UserEmail, UserPassword);
        Administrator = await CreateUserAsync(
            Role.Administrator, AdministratorEmail, AdministratorPassword);
        Analyst = await CreateUserAsync(
            Role.Analyst, AnalystEmail, AnalystPassword);
    }
}