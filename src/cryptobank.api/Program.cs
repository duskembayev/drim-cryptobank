using cryptobank.api.config;
using cryptobank.api.dal;
using cryptobank.api.Enhanced.DependencyInjection;
using cryptobank.api.utils;
using Microsoft.EntityFrameworkCore;

var appBuilder = WebApplication.CreateBuilder(args);

appBuilder.Services
    .Configure<NewsOptions>(appBuilder.Configuration.GetSection(ConfigConstants.NewsSectionKey))
    .Configure<RegisterUserOptions>(appBuilder.Configuration.GetSection(ConfigConstants.RegisterUserSectionKey))
    .AddDbContext<CryptoBankDbContext>(options =>
        options.UseNpgsql(appBuilder.Configuration.GetNpgsqlConnectionString()))
    .AddEnhancedModules()
    .AddSwaggerGen()
    .AddControllers();

var app = appBuilder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapSwagger();

await app.RestoreDatabaseAsync(500);
await app.RunAsync();