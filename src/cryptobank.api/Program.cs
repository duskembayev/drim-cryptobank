using System.Reflection;
using System.Text.Json.Serialization;
using cryptobank.api.features.accounts;
using cryptobank.api.features.news;
using cryptobank.api.features.users;
using cryptobank.api.redis;
using cryptobank.api.utils.pipeline;
using FastEndpoints.Swagger;

var thisAssembly = Assembly.GetExecutingAssembly();
var appBuilder = WebApplication.CreateBuilder(args);

appBuilder
    .AddNews()
    .AddUsers()
    .AddAccounts();

appBuilder.Services
    .AddRedis()
    .AddFastEndpoints()
    .SwaggerDocument(options =>
    {
        options.ShortSchemaNames = true;
        options.EnableJWTBearerAuth = true;
    })
    .AddMediatR(configuration =>
    {
        configuration
            .RegisterServicesFromAssembly(thisAssembly)
            .AddOpenBehavior(typeof(RequestValidationBehavior<,>));
    })
    .AddEnhancedModules(appBuilder.Configuration.GetSection("Features"))
    .AddDbContext(appBuilder.Configuration)
    .AddValidatorsFromAssembly(thisAssembly);

var app = appBuilder.Build();

app.UseProblemExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints(c =>
{
    c.Endpoints.ShortNames = true;
    c.Serializer.Options.Converters.Add(new JsonStringEnumConverter());
});
app.UseSwaggerGen();

await app.RestoreDatabaseAsync(500);
await app.RunAsync();