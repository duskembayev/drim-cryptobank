using cryptobank.api.dal.news;

namespace cryptobank.api.dto;

public interface IDtoTranslator
{
    NewsModel Translate(News news);
}