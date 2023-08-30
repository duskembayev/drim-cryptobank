using System.Text.Json.Serialization;

namespace cryptobank.api.errors.exceptions;

public sealed class ValidationException : ProblemException
{
    private readonly ImmutableArray<ValidationError> _errors;

    public ValidationException(ImmutableArray<ValidationError> errors)
        : base(message: "One or more validation failures have occurred.")
    {
        _errors = errors;
    }

    protected override int Status => StatusCodes.Status400BadRequest;
    protected override string Title => "Validation Failed";

    internal override ProblemDetails ToDetails()
    {
        var details = base.ToDetails();
        details.Extensions["errors"] = _errors;
        return details;
    }

    public record ValidationError(
        [property: JsonPropertyName("property")] string Property,
        [property: JsonPropertyName("message")] string Message,
        [property: JsonPropertyName("code")] string Code);
}