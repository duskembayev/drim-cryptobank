using System.Diagnostics;
using cryptobank.api.errors.exceptions;
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

            var details = exceptionHandlerFeature.Error switch
            {
                ProblemException problemException => problemException.ToDetails(),
                _ => new ProblemDetails
                {
                    Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/500",
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Internal Server Error",
                    Detail = "An unhandled exception has occurred while executing the request.",
                }
            };

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