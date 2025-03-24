using Microsoft.AspNetCore.Mvc;

namespace service_matrix.Controllers;

[ApiController]
[Route("[controller]")]
public class WordSearchController : ControllerBase
{
    private readonly ILogger<WordSearchController> _logger;

    public WordSearchController(ILogger<WordSearchController> logger)
    {
        _logger = logger;
    }

    [HttpPost(Name = "GetWords")]
    public async Task<Dictionary<string, Dictionary<int, Dictionary<string, string>>>> Get(SearchRequest request)
    {
        var handler = new WordSearchCommandHandler();
        var command = new WordSearchCommand(request.MaxLength, request.MinLength, request.MaxWords, request.LettersMatrix!);
        var res = await handler.Handle(command, CancellationToken.None);

        return res;
    }
 
}
