using cryptobank.api.features.users.responses;

namespace cryptobank.api.features.users.requests;

public class LoginUserRequest : IRequest<TokenResponse>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }

    public class Validator : AbstractValidator<LoginUserRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithErrorCode("users:login:email_empty")
                .WithMessage("Email is required");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithErrorCode("users:login:password_empty")
                .WithMessage("Password is required");
        }
    }
}