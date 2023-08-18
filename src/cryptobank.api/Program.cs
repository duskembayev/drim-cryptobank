using System.Reflection;
using cryptobank.api.config;
using cryptobank.api.Enhanced.DependencyInjection;
using cryptobank.dal;
using FastEndpoints;

var appBuilder = WebApplication.CreateBuilder(args);

appBuilder.Services
    .Configure<NewsOptions>(appBuilder.Configuration.GetSection(ConfigConstants.NewsSectionKey))
    .Configure<RegisterUserOptions>(appBuilder.Configuration.GetSection(ConfigConstants.RegisterUserSectionKey))
    .AddEnhancedModules()
    .AddFastEndpoints()
    .AddMediatR(configuration => configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
    .AddDbContext(appBuilder.Configuration);

var app = appBuilder.Build();

app.UseFastEndpoints();

await app.Services.RestoreDatabaseAsync(500, app.Environment.IsDevelopment());
await app.RunAsync();