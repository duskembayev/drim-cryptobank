using System.Diagnostics.CodeAnalysis;

namespace cryptobank.api.utils;

public class OperationResponse<T>
{
    [MemberNotNullWhen(true, nameof(Result))]
    [MemberNotNullWhen(false, nameof(ErrorMessage))]
    public bool Success { get; set; }
    public T? Result { get; set; }
    public string? ErrorMessage { get; set; }
}