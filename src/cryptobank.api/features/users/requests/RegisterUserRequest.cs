using cryptobank.api.features.users.config;
using cryptobank.api.features.users.services;

namespace cryptobank.api.features.users.requests;

public class RegisterUserRequest : IRequest<int>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; } = DateOnly.MinValue;

    public class Validator : AbstractValidator<RegisterUserRequest>
    {
        public Validator(
            IPasswordStrengthValidator passwordStrengthValidator,
            ITimeProvider timeProvider,
            CryptoBankDbContext dbContext,
            IOptions<RegisterUserOptions> options)
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithErrorCode("users:register:email_invalid")
                .WithMessage("Email is not valid");

            RuleFor(x => x.Email)
                .NotEmpty()
                .MustAsync(async (s, token) => !await dbContext.Users.AnyAsync(u => u.Email == s, token))
                .WithErrorCode("users:register:user_exists")
                .WithMessage("User already exists");

            RuleFor(x => x.Password)
                .NotEmpty()
                .Must(passwordStrengthValidator.Validate)
                .WithErrorCode("users:register:password_weak")
                .WithMessage("Password is not strong enough");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty()
                .Must(dateOfBirth => dateOfBirth <= timeProvider.Today.AddYears(-options.Value.MinimumAge))
                .WithErrorCode("users:register:too_young")
                .WithMessage($"User must be at least {options.Value.MinimumAge} years old");
        }
    }
}