namespace cryptobank.api.features.accounts.validators;

public class CreateAccountValidator : Validator<CreateAccountRequest>
{
    public CreateAccountValidator()
    {
        RuleFor(request => request.Currency)
            .IsInEnum()
            .WithMessage("Currency is not valid");

        RuleFor(request => request.UserId)
            .GreaterThan(0)
            .WithMessage("UserId is not valid");
    }
}