namespace cryptobank.api.errors.exceptions;

public abstract class ProblemException : Exception
{
    protected ProblemException(string? code = null, string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
        Code = code;
    }

    public string? Code { get; init; }
}