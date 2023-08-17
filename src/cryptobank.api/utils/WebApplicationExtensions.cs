using cryptobank.api.dal;
using Microsoft.EntityFrameworkCore;

namespace cryptobank.api.utils;

internal static class WebApplicationExtensions
{
    public static async Task RestoreDatabaseAsync(this WebApplication @this, int dbWarmupTimeout)
    {
        await Task.Delay(dbWarmupTimeout);

        using var serviceScope = @this.Services.CreateScope();

        var dbContext = serviceScope.ServiceProvider.GetRequiredService<CryptoBankDbContext>();
        var env = serviceScope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

        if (await dbContext.Database.EnsureCreatedAsync())
        {
            await dbContext.ApplyReferencesAsync();
            await dbContext.ApplySamplesAsync(env);
            return;
        }

        await dbContext.Database.MigrateAsync();
    }
}