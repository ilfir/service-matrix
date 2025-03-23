using Microsoft.AspNetCore.Mvc;

namespace service_matrix.Controllers;

[ApiController]
[Route("[controller]")]
public class WordSearchController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WordSearchController> _logger;

    public WordSearchController(ILogger<WordSearchController> logger)
    {
        _logger = logger;
    }

    [HttpPost(Name = "GetWords")]
    public async Task<List<string>> Get(SearchRequest request)
    {
        // var rng = new Random();
        // var result = Enumerable.Range(1, 5).Select(index => Summaries[rng.Next(Summaries.Length)]).ToArray();
        // Join the lettersMatrix into a single string
        var joinedLettersMatrix = string.Join(", ", request.lettersMatrix.SelectMany(row => row));
        // Include the joined lettersMatrix in the result

        var handler = new WordSearchCommandHandler();
        var command = new WordSearchCommand(request.MaxLength, request.MinLength, request.MaxWords, request.lettersMatrix);
        var res = await handler.Handle(command, CancellationToken.None);

        var response = new List<string> { joinedLettersMatrix, res };

        return response;
    }
 
}
