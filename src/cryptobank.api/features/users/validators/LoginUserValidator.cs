using FastEndpoints;
using FluentValidation;

namespace cryptobank.api.features.users.validators;

public class LoginUserValidator : Validator<LoginUserRequest>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");
    }
}