using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;

namespace cryptobank.api.errors;

public static class SetupExtensions
{
    public static IApplicationBuilder UseProblemExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appError => appError.Run(async context =>
        {
            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
            if (exceptionHandlerFeature is null)
                return;
            
            var details = context.RequestServices
                .GetRequiredService<HttpProblemDetailsBuilder>()
                .Build(exceptionHandlerFeature.Error);

            details.Extensions["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier;
            context.Response.StatusCode = details.Status!.Value;

            await context.Response.WriteAsJsonAsync(
                details,
                options: null,
                contentType: "application/problem+json",
                cancellationToken: context.RequestAborted);
        }));

        return app;
    }
}