namespace cryptobank.api.errors;

[Singleton]
internal class HttpProblemDetailsBuilder
{
    public ProblemDetails Build(Exception exception)
    {
        return exception switch
        {
            LogicException logicException => GetDetails(logicException),
            SecurityException securityException => GetDetails(securityException),
            ValidationException validationException => GetDetails(validationException),
            _ => GetDetails()
        };
    }

    private static ProblemDetails GetDetails()
    {
        return GetDetailsCore(
            StatusCodes.Status500InternalServerError,
            "Internal Server Error",
            "An unhandled exception has occurred while executing the request");
    }

    private static ProblemDetails GetDetails(ValidationException exception)
    {
        var details = GetDetailsCore(
            StatusCodes.Status400BadRequest,
            "Validation Failed",
            exception.Message,
            exception.Code);

        details.Extensions["errors"] = exception.Errors;
        return details;
    }

    private static ProblemDetails GetDetails(SecurityException exception)
    {
        return GetDetailsCore(
            StatusCodes.Status400BadRequest,
            "Security Validation Failed",
            exception.Message,
            exception.Code);
    }

    private static ProblemDetails GetDetails(LogicException exception)
    {
        return GetDetailsCore(
            StatusCodes.Status409Conflict,
            "Logic Error",
            exception.Message,
            exception.Code);
    }

    private static ProblemDetails GetDetailsCore(
        int status,
        string title,
        string? message = null,
        string? code = null)
    {
        var details = new ProblemDetails
        {
            Type = $"https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/{status:D}",
            Status = status,
            Title = title,
            Detail = message
        };

        if (code is not null)
            details.Extensions["code"] = code;

        return details;
    }
}