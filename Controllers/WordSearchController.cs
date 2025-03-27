using Microsoft.AspNetCore.Mvc;
using service_matrix.CommandHandlers;
using service_matrix.Commands;
using service_matrix.DTO;

namespace service_matrix.Controllers;

[ApiController]
[Route("[controller]")]
public class WordsController : ControllerBase
{
    private readonly ILogger<WordsController> _logger;

    public WordsController(ILogger<WordsController> logger)
    {
        _logger = logger;
    }

    [HttpPost("Search", Name = "Search")]
    public async Task<Dictionary<string, Dictionary<int, Dictionary<string, string>>>> Search(SearchRequest request)
    {
        var handler = new WordSearchCommandHandler();
        var command = new WordSearchCommand(request.MaxLength, request.MinLength, request.MaxWords, request.LettersMatrix!);
        var res = await handler.Handle(command, CancellationToken.None);

        return res;
    }

    /// <summary>
    /// Accept list of words and flag to
    /// include or exclude them from the search
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("Update", Name = "Update")]
    public async Task<IActionResult> Update(UpdateWordsRequest request)
    {
        var handler = new UpdateWordsCommandHandler();
        var command = new UpdateWordsCommand(request.Words, request.Include);
        var res = await handler.Handle(command, CancellationToken.None);

        return Ok(res);
    }
}
