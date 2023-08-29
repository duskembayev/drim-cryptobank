namespace cryptobank.api.errors.exceptions;

public abstract class ProblemException : Exception
{
    protected ProblemException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
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

        return details;
    }
}