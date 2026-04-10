using Microsoft.Extensions.Logging;
using System.IO;
using service_matrix.Queries;

namespace service_matrix.QueryHandlers;

/// <summary>
/// Handles getting word lists with logging and error handling.
/// </summary>
public class GetWordsQueryHandler
{
    private readonly ILogger<GetWordsQueryHandler> _logger;

    public GetWordsQueryHandler(ILogger<GetWordsQueryHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets the list of words from include or exclude file.
    /// </summary>
    /// <param name="query">The query specifying which list to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of words</returns>
    public async Task<List<string>> Handle(GetWordsQuery query, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting word list. Include={Include}", query.Include);
            var fileName = !query.Include ? "exclude.txt" : "include.txt";
            string filePath = Path.Combine(AppContext.BaseDirectory, "data", fileName);

            // Read the file lines asynchronously
            var words = await File.ReadAllLinesAsync(filePath, cancellationToken);

            return new List<string>(words);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving word list from {File}", !query.Include ? "exclude.txt" : "include.txt");
            throw;
        }
    }
}