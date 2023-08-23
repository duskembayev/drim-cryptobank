namespace cryptobank.api.db;

public static class SetupExtensions
{
    private const string ConnectionStringName = "cryptobank";

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
        return @this.GetConnectionString(ConnectionStringName)
               ?? throw new InvalidOperationException($"Connection string '{ConnectionStringName}' not found");
    }
}