using cryptobank.api.features.users.config;
using cryptobank.api.utils.environment;
using cryptobank.api.utils.security;

namespace cryptobank.api.features.users.requests;

public class RegisterUserRequest : IRequest<int>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; } = DateOnly.MinValue;

    public class Validator : AbstractValidator<RegisterUserRequest>
    {
        public Validator(
            IEmailFormatValidator emailFormatValidator,
            IPasswordStrengthValidator passwordStrengthValidator,
            ITimeProvider timeProvider,
            IOptions<RegisterUserOptions> options)
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .Must(emailFormatValidator.Validate)
                .WithMessage("Email is not valid");

            RuleFor(x => x.Password)
                .NotEmpty()
                .Must(passwordStrengthValidator.Validate)
                .WithMessage("Password is not strong enough");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty()
                .Must(dateOfBirth => dateOfBirth <= timeProvider.Today.AddYears(-options.Value.MinimumAge))
                .WithMessage($"User must be at least {options.Value.MinimumAge} years old");
        }
    }
}