namespace cryptobank.api.features.users.config;

public class AccessTokenOptions
{
    public string Issuer { get; set; } = "CryptoBank";
    public string Audience { get; set; } = "CryptoBank";
    public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(15);
    public string SecurityKey { get; set; } = string.Empty;
}