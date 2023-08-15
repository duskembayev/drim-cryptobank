using cryptobank.api;
using cryptobank.api.dal;
using Microsoft.EntityFrameworkCore;

const int dbWarmupTimeout = 500;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
    .Build();

using (var serviceScope = host.Services.CreateScope())
{
    await Task.Delay(dbWarmupTimeout);

    var dbContext = serviceScope.ServiceProvider.GetRequiredService<CryptoBankDbContext>();
    var env = serviceScope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

    if (await dbContext.Database.EnsureCreatedAsync())
        await dbContext.ApplySamplesAsync(env);
    else
        await dbContext.Database.MigrateAsync();
}

await host.RunAsync();