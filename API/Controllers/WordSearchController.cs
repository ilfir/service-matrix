using Microsoft.AspNetCore.Mvc;
using service_matrix.CommandHandlers;
using service_matrix.Commands;
using service_matrix.DTO;
using service_matrix.Helpers;
using service_matrix.Queries;
using service_matrix.QueryHandlers;

namespace service_matrix.Controllers;

/// <summary>
/// Controller for word search operations.
/// </summary>
[ApiController]
[Route("[controller]")]
public class WordsController : ControllerBase
{
    private readonly WordSearchCommandHandler _wordSearchHandler;
    private readonly UpdateWordsCommandHandler _updateWordsHandler;
    private readonly GetWordsQueryHandler _getWordsHandler;
    private readonly MergeWordsCommandHandler _mergeWordsHandler;
    private readonly LookupWordQueryHandler _lookupWordHandler;

    public WordsController(
        WordSearchCommandHandler wordSearchHandler,
        UpdateWordsCommandHandler updateWordsHandler,
        GetWordsQueryHandler getWordsHandler,
        MergeWordsCommandHandler mergeWordsHandler,
        LookupWordQueryHandler lookupWordHandler)
    {
        _wordSearchHandler = wordSearchHandler;
        _updateWordsHandler = updateWordsHandler;
        _getWordsHandler = getWordsHandler;
        _mergeWordsHandler = mergeWordsHandler;
        _lookupWordHandler = lookupWordHandler;
    }

    /// <summary>
    /// Run Word search for given matrix
    /// </summary>
    [HttpPost("Search", Name = "Search")]
    public async Task<Dictionary<string, Dictionary<int, Dictionary<string, string>>>> Search(SearchRequest request)
    {
        var command = new WordSearchCommand(request.MaxLength, request.MinLength, request.MaxWords, request.LettersMatrix!);
        var res = await _wordSearchHandler.Handle(command, CancellationToken.None);
        return res;
    }

    /// <summary>
    /// Accept list of words and flag to include or exclude them from the search
    /// </summary>
    [HttpPost("Update", Name = "Update")]
    public async Task<IActionResult> Update(UpdateWordsRequest request)
    {
        var command = new UpdateWordsCommand(request.Words, request.Include);
        var res = await _updateWordsHandler.Handle(command, CancellationToken.None);
        return Ok(res);
    }

    /// <summary>
    /// Get list of included/excluded words
    /// </summary>
    [HttpGet("List", Name = "GetList")]
    public async Task<IActionResult> GetList(bool include = true)
    {
        var query = new GetWordsQuery(include);
        var res = await _getWordsHandler.Handle(query, CancellationToken.None);
        return Ok(res);
    }

    /// <summary>
    /// Merge dictionary words with the include and exclude lists
    /// </summary>
    [HttpPost("Merge")]
    public async Task<IActionResult> MergeWords()
    {
        var res = await _mergeWordsHandler.Handle(new MergeWordsCommand(), CancellationToken.None);
        return Ok(res);
    }

    /// <summary>
    /// One off to get rid of hyphenated words or words with spaces in them
    /// </summary>
    [HttpGet("CleanMerge")]
    public async Task<IActionResult> CleanMerge()
    {
        var input = FileHelper.ReadFileAsync("resources", "merged.txt");
        var output = WordSearchHelper.CleanWords(input);
        await FileHelper.WriteFileNewContents(output, "data", "merged_cleaned.txt");
        return Ok($"BEFORE: {input.Count()} AFTER: {output.Count()}");
    }

    /// <summary>
    /// Lookup word or part of word in all dictionaries
    /// </summary>
    [HttpGet("LookupWord")]
    public async Task<IActionResult> LookupWord(string word, bool exactMatch = false)
    {
        var query = new LookupWordQuery(word, exactMatch);
        var res = await _lookupWordHandler.Handle(query, CancellationToken.None);
        return Ok(res);
    }
}
