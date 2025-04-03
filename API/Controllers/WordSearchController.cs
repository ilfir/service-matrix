using Microsoft.AspNetCore.Mvc;
using service_matrix.CommandHandlers;
using service_matrix.Commands;
using service_matrix.DTO;
using service_matrix.Helpers;
using service_matrix.Queries;
using service_matrix.QueryHandlers;

namespace service_matrix.Controllers;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("[controller]")]
public class WordsController : ControllerBase
{
    /// <summary>
    /// Run Word search for given matrix
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
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
    
    /// <summary>
    /// Get list of included/excluded words
    /// </summary>
    /// <param name="include"></param>
    /// <returns></returns>
    [HttpGet("List", Name = "GetList")]
    public async Task<IActionResult> GetList(bool include = true)
    {
        var handler = new GetWordsQueryHandler(); // Handler for retrieving words
        var query = new GetWordsQuery(include); // Query object with the 'include' flag
        var res = await handler.Handle(query, CancellationToken.None); // Process query via handler

        return Ok(res); // Return results as HTTP 200 response

    }
    
    /// <summary>
    /// Merge dictionary words with the include and exclude lists
    /// </summary>
    /// <returns></returns>
    [HttpPost("Merge")]
    public async Task<IActionResult> MergeWords()
    {
        var handler = new MergeWordsCommandHandler();
        var res = await handler.Handle(new MergeWordsCommand(), CancellationToken.None);
        
        // Return the count of new words added
        return Ok(res);
    }
    
    /// <summary>
    /// One off to get rid of hyphenated words or words with spaces in them
    /// </summary>
    /// <returns></returns>
    [HttpGet("CleanMerge")]
    public async Task<IActionResult> CleanMerge()
    {
        var input = FileHelper.ReadFileAsync("resources", "merged.txt");
        var output = WordSearchHelper.CleanWords(input);
        await FileHelper.WriteFileNewContents(output, "data", "merged_cleaned.txt");
            
        return Ok("BEFORE: " + input.Count() + " AFTER: " + output.Count());
    }

    /// <summary>
    /// Lookup word or part of word in all dictionaries
    /// </summary>
    /// <param name="word"></param>
    /// <param name="exactMatch">Exact or wild card</param>
    /// <returns></returns>
    [HttpGet("LookupWord")]
    public async Task<IActionResult> LookupWord(string word, bool exactMatch = false)
    {
        var query = new LookupWordQuery(word, exactMatch);
        var handler = new LookupWordQueryHandler();
        var res = await handler.Handle(query, CancellationToken.None);
        return Ok(res);
    }
    
}
