namespace cryptobank.api.db;

public static class SetupExtensions
{
    private const string ConnectionStringName = "postgres";

    public static async Task RestoreDatabaseAsync(this WebApplication @this)
    {
        var warmupTimeout = @this.Configuration.GetValue<int?>("WARMUP_TIMEOUT") ?? 500;
        
        if (warmupTimeout > 0)
            await Task.Delay(warmupTimeout);

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
        @this.AddDbContext<CryptoBankDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString(ConnectionStringName));
        });
        return @this;
    }
}