namespace cryptobank.api.errors.exceptions;

public class LogicException : ProblemException
{
    public LogicException(string code, string message, Exception? exception = null)
        : base(code, message, exception)
    {
    }
    
    protected override int Status => StatusCodes.Status409Conflict;
    protected override string Title => "Logic Error";
}