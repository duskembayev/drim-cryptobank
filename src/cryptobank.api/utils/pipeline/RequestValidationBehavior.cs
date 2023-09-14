using ValidationException = cryptobank.api.errors.exceptions.ValidationException;

namespace cryptobank.api.utils.pipeline;

public sealed class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IValidator<TRequest>? _validator;

    public RequestValidationBehavior(IValidator<TRequest>? validator = null)
    {
        _validator = validator;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        await ValidateCore(request, cancellationToken);
        return await next();
    }

    private async ValueTask ValidateCore(TRequest request, CancellationToken cancellationToken)
    {
        if (_validator is null)
            return;

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid)
            return;

        var errors = validationResult.Errors
            .Select(x => new ValidationException.ValidationError(x.PropertyName, x.ErrorMessage, x.ErrorCode))
            .ToImmutableArray();

        throw new ValidationException(errors);
    }
}