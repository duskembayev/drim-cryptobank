using System.Text.Json.Serialization;

namespace cryptobank.api.errors.exceptions;

public sealed class ValidationException : ProblemException
{
    public ValidationException(string property, string code, string message)
        : this(ImmutableArray.Create(new ValidationError(property, message, code)))
    {
    }
    
    public ValidationException(ImmutableArray<ValidationError> errors)
        : base(message: "One or more validation failures have occurred.")
    {
        Errors = errors;
    }

    public ImmutableArray<ValidationError> Errors { get; }

    public record ValidationError(
        [property: JsonPropertyName("property")] string Property,
        [property: JsonPropertyName("message")] string Message,
        [property: JsonPropertyName("code")] string Code);
}