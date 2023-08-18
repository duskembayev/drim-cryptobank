using cryptobank.api.config;
using cryptobank.api.Enhanced.DependencyInjection;
using cryptobank.dal;

var appBuilder = WebApplication.CreateBuilder(args);

appBuilder.Services
    .Configure<NewsOptions>(appBuilder.Configuration.GetSection(ConfigConstants.NewsSectionKey))
    .Configure<RegisterUserOptions>(appBuilder.Configuration.GetSection(ConfigConstants.RegisterUserSectionKey))
    .AddSwaggerGen()
    .AddEnhancedModules()
    .AddDbContext(appBuilder.Configuration)
    .AddControllers();

var app = appBuilder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapSwagger();

await app.Services.RestoreDatabaseAsync(
    500,
    app.Environment.IsDevelopment());

await app.RunAsync();