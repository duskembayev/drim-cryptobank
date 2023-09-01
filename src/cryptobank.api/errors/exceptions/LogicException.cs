namespace cryptobank.api.errors.exceptions;

public class LogicException : ProblemException
{
    public LogicException(string code, string message, Exception? exception = null)
        : base(code, message, exception)
    {
    }
}