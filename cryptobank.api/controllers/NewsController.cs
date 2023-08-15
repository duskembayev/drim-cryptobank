using cryptobank.api.config;
using cryptobank.api.dal.news;
using cryptobank.api.dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace cryptobank.api.controllers;

public class NewsController : ControllerBase
{
    private readonly INewsRepository _repository;
    private readonly IDtoTranslator _translator;
    private readonly IOptions<NewsConfig> _options;

    public NewsController(INewsRepository repository, IDtoTranslator translator, IOptions<NewsConfig> options)
    {
        _repository = repository;
        _translator = translator;
        _options = options;
    }
    
    [HttpGet]
    [Route("news")]
    public async Task<IActionResult> GetNewsAsync(CancellationToken cancellationToken)
    {
        var models = await _repository.ListAsync(_options.Value.ListingCapacity, cancellationToken);
        
        if (models.Count == 0)
            return NoContent();

        return Ok(models.Select(_translator.Translate));
    }
}