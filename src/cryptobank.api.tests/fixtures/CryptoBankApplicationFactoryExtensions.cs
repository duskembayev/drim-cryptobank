using cryptobank.api.db;
using cryptobank.api.features.users.domain;
using cryptobank.api.features.users.services;
using Microsoft.Extensions.DependencyInjection;

namespace cryptobank.api.tests.fixtures;

internal static class CryptoBankApplicationFactoryExtensions
{
    public static async ValueTask<User> CreateUserAsync(
        this CryptoBankApplicationFactory factory,
        string email = "harrypotter@hogwards.edu.uk",
        string password = "C@putDrac0nis",
        string roleName = Role.User)
    {
        using var scope = factory.Services.CreateScope();

        var hashAlgorithm = scope.ServiceProvider.GetRequiredService<IPasswordHashAlgorithm>();
        var dbContext = scope.ServiceProvider.GetRequiredService<CryptoBankDbContext>();
        var role = Role.Detached.GetByName(roleName);

        dbContext.Attach(role);

        var user = new User
        {
            Email = email,
            PasswordHash = await hashAlgorithm.HashAsync(password),
            Roles = {role},
            DateOfBirth = new DateOnly(1980, 7, 31),
            DateOfRegistration = new DateTime(1999, 6, 30, 4, 31, 48, DateTimeKind.Utc)
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return user;
    }

    public static async ValueTask RemoveUserAsync(this CryptoBankApplicationFactory factory, User user)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CryptoBankDbContext>();

        dbContext.Remove(user);
        await dbContext.SaveChangesAsync();
    }
}