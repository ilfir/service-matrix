using service_matrix.Commands;
using service_matrix.Helpers;

namespace service_matrix.CommandHandlers;

public class UpdateWordsCommandHandler
{
    public async Task<int> Handle(UpdateWordsCommand command, CancellationToken cancellationToken)
    {
        // Example logic to update words
        if (command.Words.Count == 0)
        {
            return 0; // No words to update
        }

        // Read All lines
        var fileName = !command.Include ? "exclude.txt" : "include.txt";
        var existingWords = FileHelper.ReadFileAsync("data", fileName);
        
        // Determine which words need to be added
        var newWords = command.Words
            .Where(word => !existingWords.Contains(word, StringComparer.OrdinalIgnoreCase))
            .ToList();
        
        if(newWords.Count == 0)
            return 0;

        newWords = newWords.Distinct().ToList();
        newWords.ForEach(x=>x.ToLower().Trim());
        
        // Save contents
        await FileHelper.WriteFileAppend(newWords, "data", fileName);
        return newWords.Count;
    }
}