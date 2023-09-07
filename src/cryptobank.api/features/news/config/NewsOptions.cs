namespace cryptobank.api.features.news.config;

[Options("News")]
public class NewsOptions
{
    public int ListingCapacity { get; set; } = 10;
}