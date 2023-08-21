using System.Reflection;
using cryptobank.api.db;
using cryptobank.api.Enhanced.DependencyInjection;
using cryptobank.api.features.news;
using cryptobank.api.features.users;
using cryptobank.api.middlewares;
using FastEndpoints;

var appBuilder = WebApplication.CreateBuilder(args);

appBuilder
    .AddNews()
    .AddUsers();

appBuilder.Services
    .AddEnhancedModules()
    .AddFastEndpoints() 
    .AddMediatR(configuration => configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
    .AddDbContext(appBuilder.Configuration);

var app = appBuilder.Build();

app.UseMiddleware<ApplicationExceptionMiddleware>();
app.UseFastEndpoints();

await app.RestoreDatabaseAsync(500);
await app.RunAsync();