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
    private readonly WordSearchCommandHandler _wordSearchCommandHandler;
    private readonly UpdateWordsCommandHandler _updateWordsCommandHandler;
    private readonly MergeWordsCommandHandler _mergeWordsCommandHandler;
    private readonly GetWordsQueryHandler _getWordsQueryHandler;
    private readonly LookupWordQueryHandler _lookupWordQueryHandler;
    private readonly FileHelper _fileHelper;

    /// <summary>
    /// WordsController
    /// </summary>
    /// <param name="wordSearchCommandHandler"></param>
    /// <param name="updateWordsCommandHandler"></param>
    /// <param name="mergeWordsCommandHandler"></param>
    /// <param name="getWordsQueryHandler"></param>
    /// <param name="lookupWordQueryHandler"></param>
    /// <param name="fileHelper"></param>
    public WordsController(WordSearchCommandHandler wordSearchCommandHandler,
        UpdateWordsCommandHandler updateWordsCommandHandler, 
        MergeWordsCommandHandler mergeWordsCommandHandler, 
        GetWordsQueryHandler getWordsQueryHandler, 
        LookupWordQueryHandler lookupWordQueryHandler,
        FileHelper fileHelper)
    {
        _wordSearchCommandHandler = wordSearchCommandHandler;
        _updateWordsCommandHandler = updateWordsCommandHandler;
        _mergeWordsCommandHandler = mergeWordsCommandHandler;
        _getWordsQueryHandler = getWordsQueryHandler;
        _lookupWordQueryHandler = lookupWordQueryHandler;
        _fileHelper = fileHelper;
    }

    /// <summary>
    /// Run Word search for given matrix
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("Search", Name = "Search")]
    public async Task<Dictionary<string, Dictionary<int, Dictionary<string, string>>>> Search(SearchRequest request)
    {
        var command = new WordSearchCommand(request.MaxLength, request.MinLength, request.MaxWords, request.LettersMatrix!);
        var res = await _wordSearchCommandHandler.Handle(command, CancellationToken.None);

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
        var command = new UpdateWordsCommand(request.Words, request.Include);
        var res = await _updateWordsCommandHandler.Handle(command, CancellationToken.None);
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
        var query = new GetWordsQuery(include); // Query object with the 'include' flag
        var res = await _getWordsQueryHandler.Handle(query, CancellationToken.None); // Process query via handler

        return Ok(res); // Return results as HTTP 200 response

    }
    /// <summary>
    /// Merge dictionary words with the include and exclude lists
    /// </summary>
    /// <returns></returns>
    [HttpPost("Merge")]
    public async Task<IActionResult> MergeWords()
    {
        var res = await _mergeWordsCommandHandler.Handle(new MergeWordsCommand(), CancellationToken.None);

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
        var input = await _fileHelper.ReadFileAsync("MergedFilePath");
        var output = WordSearchHelper.CleanWords(input);
        await _fileHelper.WriteFileNewContents(output, "MergedCleanedFilePath");

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
        var res = await _lookupWordQueryHandler.Handle(query, CancellationToken.None);
        return Ok(res);
    }

}
