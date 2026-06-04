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
    private readonly ILogger<MergeWordsCommandHandler> _logger;

       /// <summary>
       /// Initializes a new instance of the <see cref="MergeWordsCommandHandler"/> class.
       /// </summary>
       /// <param name="fileHelper">The file helper service.</param>
       /// <param name="logger">The logger.</param>
    public MergeWordsCommandHandler(IFileHelper fileHelper, ILogger<MergeWordsCommandHandler> logger)
       {
          _fileHelper = fileHelper;
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

         var definitions = _fileHelper.ReadFile("resources", "definitions.txt");
         var merged = _fileHelper.ReadFile("resources", "merged.txt");
         var includeList = _fileHelper.ReadFile("data", "include.txt");
         var excludeList = _fileHelper.ReadFile("data", "exclude.txt");

         var allWords = new HashSet<string>(definitions.Concat(merged));
         var includeSet = new HashSet<string>(includeList);
         var excludeSet = new HashSet<string>(excludeList);

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

          var result = new MergeResponse(addedCount, removedCount);
         _logger.LogInformation("Merge words completed. Added={AddedCount}, Removed={RemovedCount}", addedCount, removedCount);
         
         return result;
       }
}