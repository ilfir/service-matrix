using Microsoft.Extensions.Logging;
using service_matrix.Helpers;
using service_matrix.Commands;

namespace service_matrix.CommandHandlers;

/// <summary>
/// Handles update words commands by adding or removing words from the include/exclude lists.
/// </summary>
public class UpdateWordsCommandHandler
{
    private readonly IFileHelper _fileHelper;
    private readonly DictionaryCacheService _dictionaryCache;
    private readonly ILogger<UpdateWordsCommandHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateWordsCommandHandler"/> class.
        /// </summary>
        /// <param name="fileHelper">The file helper service.</param>
        /// <param name="dictionaryCache">The dictionary cache service.</param>
        /// <param name="logger">The logger.</param>
    public UpdateWordsCommandHandler(IFileHelper fileHelper, DictionaryCacheService dictionaryCache, ILogger<UpdateWordsCommandHandler> logger)
        {
           _fileHelper = fileHelper;
           _dictionaryCache = dictionaryCache;
           _logger = logger;
        }

         /// <summary>
         /// Handles the update words command.
         /// </summary>
         /// <param name="command">The update words command containing words and include/exclude flag.</param>
         /// <param name="cancellationToken">A cancellation token.</param>
         /// <returns>The count of words added (0 if all were duplicates).</returns>
    public async Task<int> Handle(UpdateWordsCommand command, CancellationToken cancellationToken)
        {
           _logger.LogInformation("Processing update words command with {WordCount} words, Include={Include}",
             command.Words.Count, command.Include);

          var includeList = _dictionaryCache.Include.ToList();
          var excludeList = _dictionaryCache.Exclude.ToList();

         var updatedInclude = new List<string>(includeList);
         var updatedExclude = new List<string>(excludeList);

         int addedCount = 0;

         foreach (var word in command.Words)
              {
             if (command.Include)
                  {
                 if (!updatedInclude.Contains(word))
                      {
                         updatedInclude.Add(word);
                         addedCount++;
                         _logger.LogDebug("Added word to include list: {Word}", word);
                      }
                 else
                      {
                        _logger.LogDebug("Word already in include list: {Word}", word);
                      }
                  }
             else
                  {
                 if (!updatedExclude.Contains(word))
                      {
                         updatedExclude.Add(word);
                         addedCount++;
                         _logger.LogDebug("Added word to exclude list: {Word}", word);
                      }
                 else
                      {
                        _logger.LogDebug("Word already in exclude list: {Word}", word);
                      }
                  }
              }

         if (command.Include)
              {
                _logger.LogDebug("Writing updated include list with {Count} words.", updatedInclude.Count);
             await _fileHelper.WriteFileNewContents("data", "include.txt", updatedInclude);
              }
         else
              {
                _logger.LogDebug("Writing updated exclude list with {Count} words.", updatedExclude.Count);
             await _fileHelper.WriteFileNewContents("data", "exclude.txt", updatedExclude);
              }

          _logger.LogInformation("Update words command completed. Added={AddedCount} new words.", addedCount);
         
         return addedCount;
       }
}