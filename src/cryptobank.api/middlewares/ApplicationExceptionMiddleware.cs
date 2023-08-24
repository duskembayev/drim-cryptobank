using System.Security;

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
        catch (SecurityException e)
        {
            var errorResponse = new ErrorResponse
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = e.Message
            };

            await errorResponse.ExecuteAsync(context);
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