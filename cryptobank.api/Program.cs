using cryptobank.api.config;
using cryptobank.api.dal;
using cryptobank.api.Enhanced.DependencyInjection;
using cryptobank.api.utils;
using Microsoft.EntityFrameworkCore;

var appBuilder = WebApplication.CreateBuilder(args);

appBuilder.Services
    .Configure<NewsOptions>(appBuilder.Configuration.GetSection(ConfigConstants.NewsSectionKey))
    .AddDbContext<CryptoBankDbContext>(options =>
        options.UseNpgsql(appBuilder.Configuration.GetNpgsqlConnectionString()))
    .AddEnhancedModules()
    .AddControllers();

var app = appBuilder.Build();

app.MapControllers();

await app.RestoreDatabaseAsync(500);
await app.RunAsync();