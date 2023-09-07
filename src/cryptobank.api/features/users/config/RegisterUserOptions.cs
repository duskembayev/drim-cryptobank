namespace cryptobank.api.features.users.config;

[Options("Users:RegisterUser")]
public class RegisterUserOptions
{
    public string? FallbackAdminEmail { get; set; }
    public int MinimumAge { get; set; } = 16;
}