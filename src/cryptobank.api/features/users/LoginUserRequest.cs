namespace cryptobank.api.features.users;

public class LoginUserRequest : IRequest<TokenResponse>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}