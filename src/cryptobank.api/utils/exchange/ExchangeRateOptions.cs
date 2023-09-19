namespace cryptobank.api.utils.exchange;

public class ExchangeRateOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public TimeSpan Ttl { get; set; } = TimeSpan.FromHours(1);
}