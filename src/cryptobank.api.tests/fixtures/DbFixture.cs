using cryptobank.api.db;
using cryptobank.api.features.users.domain;
using cryptobank.api.tests.extensions;
using Microsoft.Extensions.Configuration;

namespace cryptobank.api.tests.fixtures;

public class DbFixture : IAsyncLifetime
{
    public DbFixture()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false)
            .AddEnvironmentVariables()
            .Build();

        var options = new DbContextOptionsBuilder<CryptoBankDbContext>()
            .UseNpgsql(configuration.GetConnectionStringWithRndDatabase())
            .Options;

        DbContext = new CryptoBankDbContext(options);
    }

    public CryptoBankDbContext DbContext { get; }

    public async Task InitializeAsync()
    {
        await DbContext.Database.EnsureCreatedAsync();
        await DbContext.ApplyReferencesAsync();
        await DbContext.ApplySamplesAsync();
        await CreateSampleUsersAsync();
    }

    public async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();
    }

    private async Task CreateSampleUsersAsync()
    {
        DbContext.Users
            .Add(new User
            {
                Email = "bender@futurama.com",
                DateOfRegistration = new DateTime(2019, 4, 13, 11, 28, 16, DateTimeKind.Utc),
                DateOfBirth = new DateOnly(1996, 9, 4),
                Roles = { Role.Detached.User },
                PasswordHash =
                    "$argon2id$v=1$m=8162,t=40,p=8$f1DHhWRqfmwpbs8DbfJrlbYLwdrXXUQu/35DBvwVnOSEbkOdsNBLHEzwKd6lNCamlQsklnrKlIb69LN1EQV31Q==$Kdq41XSqA1C8RThSbWCZZ2ZuOI66S/Yvos1LZSOrul7Bdi7n2wJEaKF69Q+CW7MJKR3wcU3oAlwyiiHOXMU25bmEDakYy4IN7xU7DYYZlNiN0WHKH6L6mwNjED71dTfgq3ORIkL5wT1guoZzyDCtF9VmTYf2yffLg2HZaKvC7Ds="
            });

        DbContext.Users
            .Add(new User
            {
                Email = "leela@futurama.com",
                DateOfRegistration = new DateTime(2019, 4, 13, 11, 28, 16, DateTimeKind.Utc),
                DateOfBirth = new DateOnly(1996, 9, 4),
                Roles = { Role.Detached.User },
                PasswordHash =
                    "$argon2id$v=1$m=8162,t=40,p=8$f1DHhWRqfmwpbs8DbfJrlbYLwdrXXUQu/35DBvwVnOSEbkOdsNBLHEzwKd6lNCamlQsklnrKlIb69LN1EQV31Q==$Kdq41XSqA1C8RThSbWCZZ2ZuOI66S/Yvos1LZSOrul7Bdi7n2wJEaKF69Q+CW7MJKR3wcU3oAlwyiiHOXMU25bmEDakYy4IN7xU7DYYZlNiN0WHKH6L6mwNjED71dTfgq3ORIkL5wT1guoZzyDCtF9VmTYf2yffLg2HZaKvC7Ds="
            });

        DbContext.Users
            .Add(new User
            {
                Email = "fry@futurama.com",
                DateOfRegistration = new DateTime(2019, 4, 13, 11, 28, 16, DateTimeKind.Utc),
                DateOfBirth = new DateOnly(1996, 9, 4),
                Roles = { Role.Detached.User },
                PasswordHash =
                    "$argon2id$v=1$m=8162,t=40,p=8$f1DHhWRqfmwpbs8DbfJrlbYLwdrXXUQu/35DBvwVnOSEbkOdsNBLHEzwKd6lNCamlQsklnrKlIb69LN1EQV31Q==$Kdq41XSqA1C8RThSbWCZZ2ZuOI66S/Yvos1LZSOrul7Bdi7n2wJEaKF69Q+CW7MJKR3wcU3oAlwyiiHOXMU25bmEDakYy4IN7xU7DYYZlNiN0WHKH6L6mwNjED71dTfgq3ORIkL5wT1guoZzyDCtF9VmTYf2yffLg2HZaKvC7Ds="
            });

        DbContext.Users
            .Add(new User
            {
                Email = "zoidberg@futurama.com",
                DateOfRegistration = new DateTime(2019, 4, 13, 11, 28, 16, DateTimeKind.Utc),
                DateOfBirth = new DateOnly(1996, 9, 4),
                Roles = { Role.Detached.User },
                PasswordHash =
                    "$argon2id$v=1$m=8162,t=40,p=8$f1DHhWRqfmwpbs8DbfJrlbYLwdrXXUQu/35DBvwVnOSEbkOdsNBLHEzwKd6lNCamlQsklnrKlIb69LN1EQV31Q==$Kdq41XSqA1C8RThSbWCZZ2ZuOI66S/Yvos1LZSOrul7Bdi7n2wJEaKF69Q+CW7MJKR3wcU3oAlwyiiHOXMU25bmEDakYy4IN7xU7DYYZlNiN0WHKH6L6mwNjED71dTfgq3ORIkL5wT1guoZzyDCtF9VmTYf2yffLg2HZaKvC7Ds="
            });

        await DbContext.SaveChangesAsync();
    }
}