namespace cryptobank.api.features.users.validators;

public class RefreshTokenValidator : Validator<RefreshTokenRequest>
{
    public RefreshTokenValidator()
    {
        RuleFor(request => request.RefreshToken)
            .NotEmpty()
            .WithMessage("Token is not specified");
    }
}