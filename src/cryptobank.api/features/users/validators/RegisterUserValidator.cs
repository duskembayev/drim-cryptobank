using cryptobank.api.config;
using cryptobank.api.utils.environment;
using cryptobank.api.utils.security;
using FastEndpoints;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace cryptobank.api.features.users.validators;

public class RegisterUserValidator : Validator<RegisterUserRequest>
{
    public RegisterUserValidator(
        IEmailFormatValidator emailFormatValidator,
        IPasswordStrengthValidator passwordStrengthValidator,
        ITimeProvider timeProvider,
        IOptions<RegisterUserOptions> options)
    {
        RuleFor(x => x.Email)
            .Must(emailFormatValidator.Validate)
            .WithMessage("Email is not valid");

        RuleFor(x => x.Password)
            .Must(passwordStrengthValidator.Validate)
            .WithMessage("Password is not strong enough");

        RuleFor(x => x.DateOfBirth)
            .Must(dateOfBirth => dateOfBirth <= timeProvider.Today.AddYears(-options.Value.MinimumAge))
            .WithMessage($"User must be at least {options.Value.MinimumAge} years old");
    }
}