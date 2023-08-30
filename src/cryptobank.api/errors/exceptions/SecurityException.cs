namespace cryptobank.api.errors.exceptions;

public sealed class SecurityException : ProblemException
{
    public SecurityException(string code, string message, Exception? exception = null)
        : base(code, message, exception)
    {
    }

    protected override int Status => StatusCodes.Status400BadRequest;
    protected override string Title => "Security Validation Failed";
}