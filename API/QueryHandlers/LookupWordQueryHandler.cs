using Microsoft.Extensions.Logging;
using service_matrix.DTO;
using service_matrix.Helpers;
using service_matrix.Queries;

namespace service_matrix.QueryHandlers;

/// <summary>
/// Handles word lookup queries with logging and error handling.
/// </summary>
public class LookupWordQueryHandler
{
    private readonly ILogger<LookupWordQueryHandler> _logger;

    public LookupWordQueryHandler(ILogger<LookupWordQueryHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Looks up a word in the dictionary and returns results.
    /// </summary>
    /// <param name="query">The lookup query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of lookup results</returns>
    public Task<List<LookupResultResponseItem>> Handle(LookupWordQuery query, CancellationToken cancellationToken)
    {
        var result = new List<LookupResultResponseItem>();

        try
        {
            _logger.LogInformation("Looking up word: {Word}, ExactMatch={ExactMatch}",
                query.Word, query.ExactMatch);
            
            if (query.Word == null || query.Word.Length < 4)
            {
                _logger.LogWarning("Word lookup rejected: word too short (less than 4 characters)");
                throw new Exception("At least 4 chars required");
            }
            
            result.AddRange(FindWordInDictionary("resources", "definitions.txt", query, WordLocation.Dictionary));
            result.AddRange(FindWordInDictionary("resources", "merged.txt", query, WordLocation.Merged));
            result.AddRange(FindWordInDictionary("data", "include.txt", query, WordLocation.Included));
            result.AddRange(FindWordInDictionary("data", "exclude.txt", query, WordLocation.Excluded));
            
            _logger.LogDebug("Lookup completed. Found {Count} results", result.Count);
            return Task.FromResult(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during word lookup for: {Word}", query.Word);
            result.Clear();
            result.Add(new LookupResultResponseItem(e.Message, WordLocation.Error.ToString()));
            return Task.FromResult(result);
        }
    }

    private IEnumerable<LookupResultResponseItem> FindWordInDictionary(string dir, string file, LookupWordQuery query, WordLocation loc)
    {
        var list = new List<LookupResultResponseItem>();
        var dict = FileHelper.ReadFileAsync(dir, file);
        var searchWord = query.Word.ToLower().Trim();
        
        _logger.LogDebug("Searching in {File} for: {Word}", file, searchWord);
        
        foreach (var word in dict)
        {
            if ((!query.ExactMatch && word.Contains(searchWord)) || string.Equals(searchWord, word))
            {
                list.Add(new LookupResultResponseItem(word, loc.ToString()));
            }
            
            if (list.Count() > 100)
            {
                _logger.LogWarning("Too many results, narrow your search");
                throw new Exception("Too many results, narrow your search");
            }
        }

        return list;
    }
}