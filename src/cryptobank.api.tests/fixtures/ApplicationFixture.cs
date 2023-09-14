using cryptobank.api.features.users.domain;

namespace cryptobank.api.tests.fixtures;

public class ApplicationFixture : IAsyncLifetime
{
    public const string UserEmail = "user@example.com";
    public const string UserPassword = "userP@s$w0rd";
    public const string AdministratorEmail = "admin@example.com";
    public const string AdministratorPassword = "adminP@s$w0rd";
    public const string AnalystEmail = "analyst@example.com";
    public const string AnalystPassword = "analystP@s$w0rd";
    private User? _administrator;
    private User? _analyst;
    private User? _user;

    public User User
    {
        get => _user ?? throw new InvalidOperationException();
        private set => _user = value;
    }

    public User Administrator
    {
        get => _administrator ?? throw new InvalidOperationException();
        set => _administrator = value;
    }

    public User Analyst
    {
        get => _analyst ?? throw new InvalidOperationException();
        set => _analyst = value;
    }

    internal CryptoBankApplicationFactory AppFactory { get; } = new();

    async Task IAsyncLifetime.InitializeAsync()
    {
        await AppFactory.InitializeAsync();

        User = await AppFactory.CreateUserAsync(
            UserEmail, UserPassword, Role.User);
        Administrator = await AppFactory.CreateUserAsync(
            AdministratorEmail, AdministratorPassword, Role.Administrator);
        Analyst = await AppFactory.CreateUserAsync(
            AnalystEmail, AnalystPassword, Role.Analyst);
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await AppFactory.DisposeAsync();
    }

    public HttpClient CreateClient()
    {
        return AppFactory.CreateClient();
    }
}