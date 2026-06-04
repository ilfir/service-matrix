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
[Route("words")]
public class WordSearchController : ControllerBase
{
    private readonly IFileHelper _fileHelper;
    private readonly WordSearchCommandHandler _wordSearchCommandHandler;
    private readonly UpdateWordsCommandHandler _updateWordsCommandHandler;
    private readonly MergeWordsCommandHandler _mergeWordsCommandHandler;
    private readonly GetWordsQueryHandler _getWordsQueryHandler;
    private readonly LookupWordQueryHandler _lookupWordQueryHandler;

    /// <summary>
     /// Constructor with dependency injection.
     /// </summary>
     /// <param name="fileHelper">The file helper service.</param>
    public WordSearchController(IFileHelper fileHelper)
    {
        _fileHelper = fileHelper;
        _wordSearchCommandHandler = new WordSearchCommandHandler(fileHelper);
        _updateWordsCommandHandler = new UpdateWordsCommandHandler(fileHelper);
        _mergeWordsCommandHandler = new MergeWordsCommandHandler(fileHelper);
        _getWordsQueryHandler = new GetWordsQueryHandler(fileHelper);
        _lookupWordQueryHandler = new LookupWordQueryHandler(fileHelper);
    }

    /// <summary>
     /// Run Word search for given matrix
     /// </summary>
     /// <param name="request"></param>
     /// <returns></returns>
    [HttpPost("Search", Name = "Search")]
    public async Task<IActionResult> Search(SearchRequest request)
    {
        var command = new WordSearchCommand(request.MaxLength, request.MinLength, request.MaxWords, request.LettersMatrix!);
        var res = await _wordSearchCommandHandler.Handle(command, CancellationToken.None);

        return Ok(res);
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
        var query = new GetWordsQuery(include);
        var res = await _getWordsQueryHandler.Handle(query, CancellationToken.None);

        return Ok(res);
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
        var input = _fileHelper.ReadFile("resources", "merged.txt");
        var output = WordSearchHelper.CleanWords(input);
        await _fileHelper.WriteFileNewContents("data", "merged_cleaned.txt", output);

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