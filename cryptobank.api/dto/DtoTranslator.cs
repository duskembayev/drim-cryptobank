using cryptobank.api.dal.news;

namespace cryptobank.api.dto;

public sealed class DtoTranslator : IDtoTranslator
{
    public News Translate(NewsModel model) => new(model.Title, model.Content, model.Date, model.Author);
}