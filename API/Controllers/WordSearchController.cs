using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using service_matrix.CommandHandlers;
using service_matrix.Commands;
using service_matrix.DTO;
using service_matrix.Helpers;
using service_matrix.Queries;
using service_matrix.QueryHandlers;
using System.Linq;

namespace service_matrix.Controllers;

/// <summary>
/// Controller for word search operations. Provides endpoints for searching, updating, and managing word dictionaries.
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
    private readonly ILogger<WordSearchController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="WordSearchController"/> class.
    /// </summary>
    /// <param name="fileHelper">The file helper service.</param>
    /// <param name="wordSearchCommandHandler">The word search command handler.</param>
    /// <param name="updateWordsCommandHandler">The update words command handler.</param>
    /// <param name="mergeWordsCommandHandler">The merge words command handler.</param>
    /// <param name="getWordsQueryHandler">The get words query handler.</param>
    /// <param name="lookupWordQueryHandler">The lookup word query handler.</param>
    /// <param name="logger">The logger.</param>
    public WordSearchController(
        IFileHelper fileHelper,
        WordSearchCommandHandler wordSearchCommandHandler,
        UpdateWordsCommandHandler updateWordsCommandHandler,
        MergeWordsCommandHandler mergeWordsCommandHandler,
        GetWordsQueryHandler getWordsQueryHandler,
        LookupWordQueryHandler lookupWordQueryHandler,
        ILogger<WordSearchController> logger)
    {
        _fileHelper = fileHelper;
        _wordSearchCommandHandler = wordSearchCommandHandler;
        _updateWordsCommandHandler = updateWordsCommandHandler;
        _mergeWordsCommandHandler = mergeWordsCommandHandler;
        _getWordsQueryHandler = getWordsQueryHandler;
        _lookupWordQueryHandler = lookupWordQueryHandler;
        _logger = logger;
    }

    /// <summary>
    /// Run word search for given matrix.
    /// </summary>
    /// <param name="request">The search request containing matrix and parameters.</param>
    /// <returns>A dictionary of found words with their locations in the matrix.</returns>
    /// <response value="Ok">Returns when the search completes successfully.</response>
    /// <response value="BadRequest">Returns when the request is invalid.</response>
    [HttpPost("Search", Name = "Search")]
    public async Task<IActionResult> Search(SearchRequest request)
    {
        _logger.LogInformation("Processing word search request with MinLength={MinLength}, MaxLength={MaxLength}, MaxWords={MaxWords}",
          request.MinLength, request.MaxLength, request.MaxWords);

        try
        {
            // Validate model state using DataAnnotations
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                _logger.LogWarning("Search request failed validation: {Errors}", string.Join(", ", errors));
                return BadRequest(new { success = false, error = "Validation failed", details = errors });
            }

            if (request.LettersMatrix == null || request.LettersMatrix.Count == 0)
            {
                _logger.LogWarning("Search request failed validation: LettersMatrix is null or empty.");
                return BadRequest(new { success = false, error = "LettersMatrix must be provided and cannot be empty." });
            }

            foreach (var row in request.LettersMatrix)
            {
                if (row == null || row.Count == 0)
                {
                    _logger.LogWarning("Search request failed validation: A row in LettersMatrix is null or empty.");
                    return BadRequest(new { success = false, error = "Each row in LettersMatrix must be provided and cannot be empty." });
                }
            }

            var command = new WordSearchCommand(request.MaxLength, request.MinLength, request.MaxWords, request.LettersMatrix!);
            var res = await _wordSearchCommandHandler.Handle(command, CancellationToken.None);

            _logger.LogInformation("Word search completed successfully, found {Count} words.", res.Count);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during word search for matrix size {MatrixSize}",
              request.LettersMatrix?.Count ?? 0);
            return StatusCode(500, new { success = false, error = "An unexpected error occurred during word search." });
        }
    }

    /// <summary>
    /// Accept list of words and flag to include or exclude them from the search.
    /// </summary>
    /// <param name="request">The update request containing words and include/exclude flag.</param>
    /// <returns>A confirmation of the update operation.</returns>
    /// <response value="Ok">Returns when the update completes successfully.</response>
    /// <response value="BadRequest">Returns when the request is invalid.</response>
    [HttpPost("Update", Name = "Update")]
    public async Task<IActionResult> Update(UpdateWordsRequest request)
    {
        _logger.LogInformation("Processing update request for {WordCount} words with Include={Include}",
          request.Words?.Count ?? 0, request.Include);

        try
        {
            // Validate model state using DataAnnotations
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                _logger.LogWarning("Update request failed validation: {Errors}", string.Join(", ", errors));
                return BadRequest(new { success = false, error = "Validation failed", details = errors });
            }

            var command = new UpdateWordsCommand(request.Words, request.Include);
            var res = await _updateWordsCommandHandler.Handle(command, CancellationToken.None);

            _logger.LogInformation("Update completed successfully for {WordCount} words.", request.Words.Count);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during update operation.");
            return StatusCode(500, new { success = false, error = "An unexpected error occurred during update." });
        }
    }

    /// <summary>
    /// Get list of included or excluded words.
    /// </summary>
    /// <param name="include">True to get included words, false for excluded words.</param>
    /// <returns>A list of words matching the specified filter.</returns>
    /// <response value="Ok">Returns when the operation completes successfully.</response>
    [HttpGet("List", Name = "GetList")]
    public async Task<IActionResult> GetList(bool include = true)
    {
        _logger.LogInformation("Getting {Include} words.", include ? "included" : "excluded");

        try
        {
            var query = new GetWordsQuery(include);
            var res = await _getWordsQueryHandler.Handle(query, CancellationToken.None);

            _logger.LogInformation("GetList completed successfully, returned {Count} words.", res.Count());
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting {Include} words.", include ? "included" : "excluded");
            return StatusCode(500, new { success = false, error = "An unexpected error occurred while retrieving word list." });
        }
    }

    /// <summary>
    /// Merge dictionary words with the include and exclude lists.
    /// </summary>
    /// <returns>A response containing the count of added and removed words.</returns>
    /// <response value="Ok">Returns when the merge completes successfully.</response>
    [HttpPost("Merge")]
    public async Task<IActionResult> MergeWords()
    {
        _logger.LogInformation("Processing word merge operation.");

        try
        {
            var res = await _mergeWordsCommandHandler.Handle(new MergeWordsCommand(), CancellationToken.None);

            _logger.LogInformation("Merge completed successfully. Added={AddedCount}, Removed={RemovedCount}",
              res.AddedCount, res.RemovedCount);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during word merge operation.");
            return StatusCode(500, new { success = false, error = "An unexpected error occurred during merge." });
        }
    }

    /// <summary>
    /// Clean merge: filter and sort the merged dictionary file.
    /// </summary>
    /// <returns>A confirmation message with before/after word counts.</returns>
    /// <response value="Ok">Returns when the clean merge completes successfully.</response>
    [HttpGet("CleanMerge")]
    public async Task<IActionResult> CleanMerge()
    {
        _logger.LogInformation("Processing clean merge operation.");

        try
        {
            var input = _fileHelper.ReadFile("resources", "merged.txt");
            var output = WordSearchHelper.CleanWords(input);
            await _fileHelper.WriteFileNewContents("data", "merged_cleaned.txt", output);

            var beforeCount = input.Count();
            var afterCount = output.Count();
            _logger.LogInformation("CleanMerge completed. Before: {Before} words, After: {After} words.", beforeCount, afterCount);
            return Ok(new { success = true, message = $"BEFORE: {beforeCount} words, AFTER: {afterCount} words." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during clean merge operation.");
            return StatusCode(500, new { success = false, error = "An unexpected error occurred during clean merge." });
        }
    }

    /// <summary>
    /// Lookup word or part of word in all dictionaries.
    /// </summary>
    /// <param name="word">The word or partial word to search for.</param>
    /// <param name="exactMatch">True for exact match, false for wildcard search.</param>
    /// <returns>A list of matching word definitions.</returns>
    /// <response value="Ok">Returns when the lookup completes successfully.</response>
    /// <response value="BadRequest">Returns when the word parameter is missing.</response>
    [HttpGet("LookupWord")]
    public async Task<IActionResult> LookupWord(string? word, bool exactMatch = false)
    {
        _logger.LogInformation("Looking up word '{Word}' with ExactMatch={ExactMatch}", word, exactMatch);

        try
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                _logger.LogWarning("LookupWord request failed validation: word parameter is empty.");
                return BadRequest(new { success = false, error = "Word parameter must be provided and cannot be empty." });
            }

            var query = new LookupWordQuery(word!, exactMatch);
            var res = await _lookupWordQueryHandler.Handle(query, CancellationToken.None);

            _logger.LogInformation("LookupWord completed successfully, found {Count} results.", res?.Count ?? 0);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during lookup for word '{Word}'.", word);
            return StatusCode(500, new { success = false, error = "An unexpected error occurred during word lookup." });
        }
    }
}
