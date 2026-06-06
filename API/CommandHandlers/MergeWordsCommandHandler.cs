using Microsoft.Extensions.Logging;
using service_matrix.Helpers;
using service_matrix.Commands;
using service_matrix.DTO;

namespace service_matrix.CommandHandlers;

/// <summary>
/// Handles merge words commands by combining dictionary files and applying include/exclude filters.
/// </summary>
public class MergeWordsCommandHandler
{
    private readonly IFileHelper _fileHelper;
    private readonly DictionaryCacheService _dictionaryCache;
    private readonly ILogger<MergeWordsCommandHandler> _logger;

          /// <summary>
          /// Initializes a new instance of the <see cref="MergeWordsCommandHandler"/> class.
          /// </summary>
          /// <param name="fileHelper">The file helper service.</param>
          /// <param name="dictionaryCache">The dictionary cache service.</param>
          /// <param name="logger">The logger.</param>
    public MergeWordsCommandHandler(IFileHelper fileHelper, DictionaryCacheService dictionaryCache, ILogger<MergeWordsCommandHandler> logger)
          {
             _fileHelper = fileHelper;
             _dictionaryCache = dictionaryCache;
             _logger = logger;
          }

           /// <summary>
           /// Handles the merge words command.
           /// </summary>
           /// <param name="command">The merge words command (unused, for future extensibility).</param>
           /// <param name="cancellationToken">A cancellation token.</param>
           /// <returns>A MergeResponse containing the count of added and removed words.</returns>
    public async Task<MergeResponse> Handle(MergeWordsCommand command, CancellationToken cancellationToken)
          {
             _logger.LogInformation("Starting merge words operation.");

             var allWords = new HashSet<string>(_dictionaryCache.Definitions.Concat(_dictionaryCache.Merged), StringComparer.OrdinalIgnoreCase);
             var includeSet = _dictionaryCache.Include;
             var excludeSet = _dictionaryCache.Exclude;

             var mergedWords = new HashSet<string>(allWords);
             foreach (var word in includeSet)
                   {
                 mergedWords.Add(word);
                     _logger.LogDebug("Added word from include list: {Word}", word);
                   }

             var removedCount = 0;
             foreach (var word in excludeSet)
                   {
                 if (mergedWords.Remove(word))
                       {
                         removedCount++;
                         _logger.LogDebug("Removed word from exclude list: {Word}", word);
                       }
                   }

             var addedCount = mergedWords.Count - (allWords.Union(includeSet).Count());
             if (addedCount < 0) addedCount = 0;

               _logger.LogDebug("Writing merged results with {Count} words to resources/merged.txt.", mergedWords.Count);
             await _fileHelper.WriteFileNewContents("resources", "merged.txt", mergedWords);

             // Refresh the cache after writing so subsequent requests see updated data
             _dictionaryCache.Refresh(_fileHelper);

               var result = new MergeResponse(addedCount, removedCount);
               _logger.LogInformation("Merge words completed. Added={AddedCount}, Removed={RemovedCount}", addedCount, removedCount);

             return result;
          }
}
