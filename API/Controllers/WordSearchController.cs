using Microsoft.AspNetCore.Mvc;
using service_matrix.CommandHandlers;
using service_matrix.Commands;
using service_matrix.DTO;
using service_matrix.Queries;
using service_matrix.QueryHandlers;

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
    
    [HttpGet("List", Name = "GetList")]
    public async Task<IActionResult> GetList(bool include = true)
    {
        var handler = new GetWordsQueryHandler(); // Handler for retrieving words
        var query = new GetWordsQuery(include); // Query object with the 'include' flag
        var res = await handler.Handle(query, CancellationToken.None); // Process query via handler

        return Ok(res); // Return results as HTTP 200 response

    }
    
    [HttpPost("Merge")]
    public async Task<IActionResult> MergeWords()
    {
        // Read the contents of the files
        var includeWords = await System.IO.File.ReadAllLinesAsync(IncludeFilePath);
        var dictionaryWords = new HashSet<string>(await System.IO.File.ReadAllLinesAsync(DictionaryFilePath));
        // Initialize a list to hold new words to be added
        List<string> newWords = new List<string>();
        // Iterate over include words and add them to dictionary if not already present
        foreach (var word in includeWords)
        {
            if (!dictionaryWords.Contains(word))
            {
                newWords.Add(word);
            }
        }
        // Add new words to dictionary
        if (newWords.Count > 0)
        {
            await System.IO.File.AppendAllLinesAsync(DictionaryFilePath, newWords);
        }
        // Remove successfully added words from include.txt
        var remainingWords = includeWords.Except(newWords).ToArray();
        await System.IO.File.WriteAllLinesAsync(IncludeFilePath, remainingWords);
        // Return the count of new words added
        return Ok(new { CountAdded = newWords.Count });
    }
    
}
