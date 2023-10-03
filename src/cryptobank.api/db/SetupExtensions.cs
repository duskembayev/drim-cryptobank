using static cryptobank.api.db.DbConstants;

namespace cryptobank.api.db;

public static class SetupExtensions
{
    public static async Task RestoreDatabaseAsync(this WebApplication @this)
    {
        using var scope = @this.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CryptoBankDbContext>();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<DbOptions>>();

        if (!options.Value.RestoreEnabled)
            return;
        
        if (options.Value.RestoreTimeout > TimeSpan.Zero)
            await Task.Delay(options.Value.RestoreTimeout);

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
        @this
            .AddOptions<DbOptions>()
            .BindConfiguration("Database");

        @this.AddDbContext<CryptoBankDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString(ConnectionStringName));
        });

        return @this;
    }
}