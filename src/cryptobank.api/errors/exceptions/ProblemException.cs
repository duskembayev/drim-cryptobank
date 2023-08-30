namespace cryptobank.api.errors.exceptions;

public abstract class ProblemException : Exception
{
    private readonly string? _code;

    protected ProblemException(string? code = null, string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
        _code = code;
    }

    protected virtual string Type => $"https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/{Status:D}";
    protected abstract int Status { get; }
    protected abstract string Title { get; }

    internal virtual ProblemDetails ToDetails()
    {
        var details = new ProblemDetails
        {
            Type = Type,
            Status = Status,
            Title = Title,
            Detail = Message
        };

        if (_code is not null)
            details.Extensions["code"] = _code;

        return details;
    }
}