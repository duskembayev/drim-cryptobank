using cryptobank.api.dal.news;

namespace cryptobank.api.dto;

public interface IDtoTranslator
{
    News Translate(NewsModel model);
}