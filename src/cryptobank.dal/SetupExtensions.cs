using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace cryptobank.dal;

public static class SetupExtensions
{
    public static async Task RestoreDatabaseAsync(this IServiceProvider @this, int dbWarmupTimeout, bool withSamples)
    {
        await Task.Delay(dbWarmupTimeout);

        using var serviceScope = @this.CreateScope();

        var dbContext = serviceScope.ServiceProvider.GetRequiredService<CryptoBankDbContext>();

        if (await dbContext.Database.EnsureCreatedAsync())
        {
            await dbContext.ApplyReferencesAsync();

            if (withSamples)
                await dbContext.ApplySamplesAsync();

            return;
        }

        await dbContext.Database.MigrateAsync();
    }

    public static IServiceCollection AddDbContext(this IServiceCollection @this, IConfiguration configuration)
    {
        var npgsqlConnectionString = configuration.GetNpgsqlConnectionString();
        @this.AddDbContext<CryptoBankDbContext>(options => options.UseNpgsql(npgsqlConnectionString));
        return @this;
    }

    private static string GetNpgsqlConnectionString(this IConfiguration @this)
    {
        var baseConnectionString = @this.GetConnectionString(DbConfigConstants.DbConnectionStringName);
        var dbHost = @this.GetSection(DbConfigConstants.DbHostConfigKey).Value;
        var dbPassword = @this.GetSection(DbConfigConstants.DbPasswordConfigKey).Value;
        var dbStringBuilder = new NpgsqlConnectionStringBuilder(baseConnectionString);

        if (!string.IsNullOrEmpty(dbHost))
            dbStringBuilder.Host = dbHost;

        if (!string.IsNullOrEmpty(dbPassword))
            dbStringBuilder.Password = dbPassword;

        return dbStringBuilder.ConnectionString;
    }
}