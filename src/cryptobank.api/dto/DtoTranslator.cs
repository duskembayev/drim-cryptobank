using cryptobank.dal.news;
using Enhanced.DependencyInjection;

namespace cryptobank.api.dto;

[ContainerEntry(ServiceLifetime.Singleton, typeof(IDtoTranslator))]
public sealed class DtoTranslator : IDtoTranslator
{
    public NewsModel Translate(News news) => new(news.Title, news.Content, news.Date, news.Author);
}