using service_matrix.Commands;

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

        var fileName = !command.Include ? "exclude.txt" : "include.txt";
        string filePath = Path.Combine(AppContext.BaseDirectory, "data", fileName);
        // Read All lines
        var existingWords = File.ReadLines(filePath);
        
        // Determine which words need to be added
        var newWords = command.Words
            .Where(word => !existingWords.Contains(word, StringComparer.OrdinalIgnoreCase))
            .ToList();
        
        if(newWords.Count == 0)
            return 0;

        newWords = newWords.Distinct().ToList();
        newWords.ForEach(x=>x.ToLower().Trim());

        // Open the file for editing (reading and writing)
        int entryCounter = 0;
        await using FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        fs.Seek(0, SeekOrigin.End);
        // Write new content
        await using StreamWriter writer = new StreamWriter(fs);
        foreach (var word in newWords.Distinct())
        {
            await writer.WriteLineAsync(word.ToLower());
            entryCounter++;
        }
        return entryCounter;
    }
}