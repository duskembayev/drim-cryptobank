using cryptobank.api.features.users.models;

namespace cryptobank.api.features.users.endpoints.loginUser;

public class LoginUserRequest : IRequest<TokenModel>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }

    public class Validator : AbstractValidator<LoginUserRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithErrorCode("users:login:email_empty")
                .WithMessage("Email is required");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithErrorCode("users:login:password_empty")
                .WithMessage("Password is required");
        }
    }
}