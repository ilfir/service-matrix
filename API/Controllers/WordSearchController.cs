using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using service_matrix.CommandHandlers;
using service_matrix.Commands;
using service_matrix.DTO;
using service_matrix.Helpers;
using service_matrix.Queries;
using service_matrix.QueryHandlers;

namespace service_matrix.Controllers;

/// <summary>
/// Main controller for word search operations, with integrated error handling and logging.
/// </summary>
[ApiController]
[Route("[controller]")]
public class WordsController : ControllerBase
{
    private readonly ILogger<WordsController> _logger;
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
        LookupWordQueryHandler lookupWordHandler,
        ILogger<WordsController> logger)
    {
        _wordSearchHandler = wordSearchHandler;
        _updateWordsHandler = updateWordsHandler;
        _getWordsHandler = getWordsHandler;
        _mergeWordsHandler = mergeWordsHandler;
        _lookupWordHandler = lookupWordHandler;
        _logger = logger;
    }

    [HttpPost("Search", Name = "Search")]
    public async Task<Dictionary<string, Dictionary<int, Dictionary<string, string>>>> Search(SearchRequest request)
    {
        try
        {
            _logger.LogInformation("Search request received with MaxLength={MaxLength} MinLength={MinLength} MaxWords={MaxWords}", request.MaxLength, request.MinLength, request.MaxWords);
            var command = new WordSearchCommand(request.MaxLength, request.MinLength, request.MaxWords, request.LettersMatrix!);
            var res = await _wordSearchHandler.Handle(command, CancellationToken.None);
            return res;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Search operation");
            return new();
        }
    }

    [HttpPost("Update", Name = "Update")]
    public async Task<IActionResult> Update(UpdateWordsRequest request)
    {
        try
        {
            var command = new UpdateWordsCommand(request.Words, request.Include);
            var res = await _updateWordsHandler.Handle(command, CancellationToken.None);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Update operation");
            return StatusCode(500, "Error updating words");
        }
    }

    [HttpGet("List", Name = "GetList")]
    public async Task<IActionResult> GetList(bool include = true)
    {
        try
        {
            var query = new GetWordsQuery(include);
            var res = await _getWordsHandler.Handle(query, CancellationToken.None);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during GetList operation");
            return StatusCode(500, "Error retrieving word list");
        }
    }

    [HttpPost("Merge")]
    public async Task<IActionResult> MergeWords()
    {
        try
        {
            var res = await _mergeWordsHandler.Handle(new MergeWordsCommand(), CancellationToken.None);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during MergeWords operation");
            return StatusCode(500, "Error merging words");
        }
    }

    [HttpGet("CleanMerge")]
    public async Task<IActionResult> CleanMerge()
    {
        try
        {
            var input = FileHelper.ReadFileAsync("resources", "merged.txt");
            var output = WordSearchHelper.CleanWords(input);
            await FileHelper.WriteFileNewContents(output, "data", "merged_cleaned.txt");
            return Ok($"BEFORE: {input.Count()} AFTER: {output.Count()}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during CleanMerge operation");
            return StatusCode(500, "Error cleaning merge file");
        }
    }

    [HttpGet("LookupWord")]
    public async Task<IActionResult> LookupWord(string word, bool exactMatch = false)
    {
        try
        {
            var query = new LookupWordQuery(word, exactMatch);
            var res = await _lookupWordHandler.Handle(query, CancellationToken.None);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during LookupWord operation");
            return StatusCode(500, "Error looking up word");
        }
    }
}
