namespace cryptobank.api.features.users.config;

[Options("Users:RefreshToken")]
public class RefreshTokenOptions
{
    public TimeSpan Expiration { get; set; } = TimeSpan.FromDays(5);
    public int TokenSize { get; set; } = 32;
}