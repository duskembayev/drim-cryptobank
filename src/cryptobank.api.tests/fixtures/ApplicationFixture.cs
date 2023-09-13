using cryptobank.api.features.users.domain;

namespace cryptobank.api.tests.fixtures;

public class ApplicationFixture : IAsyncLifetime
{
    public const string UserEmail = "user@example.com";
    public const string UserPassword = "userP@s$w0rd";
    public const string AdministratorEmail = "admin@example.com";
    public const string AdministratorPassword = "adminP@s$w0rd";
    private User? _administrator;
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

    internal CryptoBankApplicationFactory AppFactory { get; } = new();


    async Task IAsyncLifetime.InitializeAsync()
    {
        await AppFactory.InitializeAsync();

        User = await AppFactory.CreateUserAsync(
            UserEmail, UserPassword, roleName: Role.User);
        Administrator = await AppFactory.CreateUserAsync(
            AdministratorEmail, AdministratorPassword, roleName: Role.Administrator);
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