using System.Diagnostics.CodeAnalysis;

namespace cryptobank.api.utils;

public class GenericResponse<T>
{
    [MemberNotNullWhen(true, nameof(Result))]
    [MemberNotNullWhen(false, nameof(ErrorMessage))]
    public bool Success { get; set; }
    public T? Result { get; set; }
    public string? ErrorMessage { get; set; }
}