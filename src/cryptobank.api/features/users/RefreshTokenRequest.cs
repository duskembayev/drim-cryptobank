namespace cryptobank.api.features.users;

public class RefreshTokenRequest : IRequest<TokenResponse>
{
    public string RefreshToken { get; set; } = string.Empty;
}