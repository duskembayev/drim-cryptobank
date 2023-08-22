namespace cryptobank.api.features.news.domain;

public class News
{
    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTime Date { get; init; } = DateTime.UnixEpoch;
    public string Author { get; init; } = string.Empty;
}