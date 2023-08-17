namespace cryptobank.api.config;

public class RegisterUserOptions
{
    public string? FallbackAdminEmail { get; set; }
    public int PasswordSaltSize { get; set; } = 64;
    public int PasswordHashSize { get; set; } = 128;
    public int MinimumAge { get; set; } = 16;
}