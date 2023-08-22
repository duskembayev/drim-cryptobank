using Npgsql;

namespace cryptobank.api.db;

public static class SetupExtensions
{
    public static async Task RestoreDatabaseAsync(this WebApplication @this, int dbWarmupTimeout)
    {
        await Task.Delay(dbWarmupTimeout);

        using var serviceScope = @this.Services.CreateScope();

        var dbContext = serviceScope.ServiceProvider.GetRequiredService<CryptoBankDbContext>();

        if (await dbContext.Database.EnsureCreatedAsync())
        {
            await dbContext.ApplyReferencesAsync();

            if (@this.Environment.IsDevelopment())
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