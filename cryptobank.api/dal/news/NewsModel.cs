namespace cryptobank.api.dal.news;

public class NewsModel
{
    internal NewsModel()
    {
    }

    public NewsModel(string mrn, string title, string content, DateTime date, string author)
    {
        Mrn = mrn;
        Title = title;
        Content = content;
        Date = date;
        Author = author;
    }

    public string Mrn { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTime Date { get; init; } = DateTime.UnixEpoch;
    public string Author { get; init; } = string.Empty;
}