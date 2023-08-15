using cryptobank.api;
using cryptobank.api.dal;
using Microsoft.EntityFrameworkCore;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
    .Build();

using (var serviceScope = host.Services.CreateScope())
{
    await Task.Delay(1_000);

    var dbContext = serviceScope.ServiceProvider.GetRequiredService<CryptoBankDbContext>();
    var env = serviceScope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

    if (await dbContext.Database.EnsureCreatedAsync())
        await dbContext.ApplySamplesAsync(env);
    else
        await dbContext.Database.MigrateAsync();
}

await host.RunAsync();