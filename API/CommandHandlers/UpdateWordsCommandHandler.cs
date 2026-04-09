using Microsoft.Extensions.Logging;
using service_matrix.Commands;
using service_matrix.DTO;
using service_matrix.Helpers;

namespace service_matrix.CommandHandlers;

/// <summary>
/// Handles updating words in include/exclude lists with proper logging.
/// </summary>
public class UpdateWordsCommandHandler
{
    private readonly ILogger<UpdateWordsCommandHandler> _logger;

    public UpdateWordsCommandHandler(ILogger<UpdateWordsCommandHandler> logger)
    {
        _logger = logger;
    }

    public async Task<int> Handle(UpdateWordsCommand command, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating words. Count={Count} Include={Include}", command.Words.Count, command.Include);
            if (command.Words.Count == 0)
            {
                _logger.LogInformation("No words provided to update.");
                return 0;
            }

            var fileName = command.Include ? "include.txt" : "exclude.txt";
            var existingWords = FileHelper.ReadFileAsync("data", fileName);

            var newWords = command.Words
                .Where(word => !existingWords.Contains(word, StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (newWords.Count == 0)
            {
                _logger.LogInformation("No new words to add.");
                return 0;
            }

            var trimmedLoweredList = new List<string>();
            foreach (var newWord in newWords.Distinct())
            {
                trimmedLoweredList.Add(newWord.ToLower().Trim());
            }

            await FileHelper.WriteFileAppend(trimmedLoweredList, "data", fileName);
            _logger.LogInformation("Added {Count} new words to {File}", trimmedLoweredList.Count, fileName);
            return trimmedLoweredList.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating words.");
            throw;
        }
    }
}
