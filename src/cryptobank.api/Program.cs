﻿using System.Reflection;
using cryptobank.api.Enhanced.DependencyInjection;
using cryptobank.api.features.news;
using cryptobank.api.features.users;
using cryptobank.api.middlewares;
using FastEndpoints.Swagger;
using NJsonSchema;

var appBuilder = WebApplication.CreateBuilder(args);

appBuilder
    .AddUsers()
    .AddNews();

appBuilder.Services
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
app.UseFastEndpoints(c => c.Endpoints.ShortNames = true);
app.UseSwaggerGen();

await app.RestoreDatabaseAsync(500);
await app.RunAsync();