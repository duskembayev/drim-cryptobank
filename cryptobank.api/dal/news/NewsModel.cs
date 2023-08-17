namespace cryptobank.api.dal.news;

public class NewsModel
{
    internal NewsModel()
    {
    }

    public NewsModel(string id, string title, string content, DateTime date, string author)
    {
        Id = id;
        Title = title;
        Content = content;
        Date = date;
        Author = author;
    }

    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTime Date { get; init; } = DateTime.UnixEpoch;
    public string Author { get; init; } = string.Empty;
}