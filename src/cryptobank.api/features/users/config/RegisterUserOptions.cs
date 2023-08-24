namespace cryptobank.api.features.users.config;

public class RegisterUserOptions
{
    public string? FallbackAdminEmail { get; set; }
    public int MinimumAge { get; set; } = 16;
}