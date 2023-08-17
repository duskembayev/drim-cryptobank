using cryptobank.api.dal.news;
using Enhanced.DependencyInjection;

namespace cryptobank.api.dto;

[ContainerEntry(ServiceLifetime.Singleton, typeof(IDtoTranslator))]
public sealed class DtoTranslator : IDtoTranslator
{
    public News Translate(NewsModel model) => new(model.Title, model.Content, model.Date, model.Author);
}