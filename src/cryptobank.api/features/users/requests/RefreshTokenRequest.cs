using cryptobank.api.features.users.responses;

namespace cryptobank.api.features.users.requests;

public class RefreshTokenRequest : IRequest<TokenResponse>
{
    public string RefreshToken { get; set; } = string.Empty;

    public class Validator : AbstractValidator<RefreshTokenRequest>
    {
        public Validator()
        {
            RuleFor(request => request.RefreshToken)
                .NotEmpty()
                .WithMessage("Token is not specified");
        }
    }
}