using Microsoft.Extensions.Logging;
using service_matrix.Commands;
using service_matrix.DTO;
using service_matrix.Helpers;

namespace service_matrix.CommandHandlers;

/// <summary>
/// Handles merging words into the dictionary with logging and error handling.
/// </summary>
public class MergeWordsCommandHandler
{
    private readonly ILogger<MergeWordsCommandHandler> _logger;

    public MergeWordsCommandHandler(ILogger<MergeWordsCommandHandler> logger)
    {
        _logger = logger;
    }

    public async Task<MergeResponse> Handle(MergeWordsCommand cmd, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting merge words operation");
            var removedCounter = 0;

            var includes = FileHelper.ReadFileAsync("data", "include.txt").ToList();
            var dictionary = FileHelper.ReadFileAsync("resources", "definitions.txt").ToHashSet();
            var merged = FileHelper.ReadFileAsync("resources", "merged.txt");
            dictionary.UnionWith(merged);

            var mergedList = new List<string>();
            foreach (var include in includes)
            {
                if (include.Length < 4 || include.IndexOf('-') > -1 || include.IndexOf(' ') > -1)
                {
                    continue;
                }

                var includeFormatted = include.ToLower().Trim().Replace('ё', 'е');
                if (!dictionary.Contains(includeFormatted))
                {
                    mergedList.Add(includeFormatted);
                }
            }
            includes = includes.Except(mergedList).ToList();

            // Save all files
            await FileHelper.WriteFileNewContents(mergedList, "data", "mergeable_definitions.txt");
            await FileHelper.WriteFileNewContents(includes, "data", "include.txt");

            _logger.LogInformation("Merge words completed: {Count} new words merged", mergedList.Count);
            var res = new MergeResponse(mergedList.Count, removedCounter);
            return res;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during merge words operation");
            throw;
        }
    }
}
