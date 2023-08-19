using Enhanced.DependencyInjection;
using FastEndpoints;

namespace cryptobank.api.middlewares;

[ContainerEntry(ServiceLifetime.Singleton)]
public class ApplicationExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ApplicationException e)
        {
            var errorResponse = new ErrorResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = e.Message
            };

            await errorResponse.ExecuteAsync(context);
        }
    }
}