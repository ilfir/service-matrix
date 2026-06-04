using service_matrix.Commands;
using service_matrix.Helpers;

namespace service_matrix.CommandHandlers;

/// <summary>
/// Handles update words commands using injected file service.
/// </summary>
public class UpdateWordsCommandHandler
{
    private readonly IFileHelper _fileHelper;

      /// <summary>
      /// Constructor with dependency injection.
      /// </summary>
      /// <param name="fileHelper">The file helper service.</param>
    public UpdateWordsCommandHandler(IFileHelper fileHelper)
       {
          _fileHelper = fileHelper;
       }

        /// <summary>
         /// 
         /// </summary>
         /// <param name="command"></param>
         /// <param name="cancellationToken"></param>
         /// <returns></returns>
    public async Task<int> Handle(UpdateWordsCommand command, CancellationToken cancellationToken)
        {
           // Example logic to update words
           if (command.Words.Count == 0)
             {
            return 0; // No words to update
             }

            // Read All lines
           var fileName = !command.Include ? "exclude.txt" : "include.txt";
           var existingWords = _fileHelper.ReadFile("data", fileName);
           
            // Determine which words need to be added
           var newWords = command.Words
                .Where(word => !existingWords.Contains(word, StringComparer.OrdinalIgnoreCase))
                .ToList();
           
           if(newWords.Count == 0)
               return 0;

           var trimmedLoweredList = new List<string>();
           foreach (var newWord in newWords.Distinct().ToList())
             {
            trimmedLoweredList.Add(newWord.ToLower().Trim());
             }
           
            // Save contents
           await _fileHelper.WriteFileAppend("data", fileName, trimmedLoweredList);
           return trimmedLoweredList.Count;
        }
}