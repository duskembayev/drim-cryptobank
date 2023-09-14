using cryptobank.api.features.users.models;

namespace cryptobank.api.features.users.endpoints.refreshToken;

public class RefreshTokenRequest : IRequest<TokenModel>
{
    public string RefreshToken { get; set; } = string.Empty;

    public class Validator : AbstractValidator<RefreshTokenRequest>
    {
        public Validator()
        {
            RuleFor(request => request.RefreshToken)
                .NotEmpty()
                .WithErrorCode("users:refresh_token:token_empty")
                .WithMessage("Token is not specified");
        }
    }
}