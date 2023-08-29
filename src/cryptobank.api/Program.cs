using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using cryptobank.api.Enhanced.DependencyInjection;
using cryptobank.api.features.accounts;
using cryptobank.api.features.news;
using cryptobank.api.features.users;
using cryptobank.api.middlewares;
using cryptobank.api.redis;
using FastEndpoints.Swagger;

var appBuilder = WebApplication.CreateBuilder(args);

appBuilder
    .AddNews()
    .AddUsers()
    .AddAccounts();

appBuilder.Services
    .AddRedis()
    .AddEnhancedModules()
    .AddFastEndpoints()
    .SwaggerDocument(options =>
    {
        options.ShortSchemaNames = true;
        options.EnableJWTBearerAuth = true;
    })
    .AddMediatR(configuration => configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
    .AddDbContext(appBuilder.Configuration);

var app = appBuilder.Build();

app.UseMiddleware<ApplicationExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints(c =>
{
    c.Endpoints.ShortNames = true;
    c.Serializer.Options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false));
});
app.UseSwaggerGen();

await app.RestoreDatabaseAsync(500);
await app.RunAsync();