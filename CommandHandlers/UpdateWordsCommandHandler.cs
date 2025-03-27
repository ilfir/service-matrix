using service_matrix.Commands;

namespace service_matrix.CommandHandlers;

public class UpdateWordsCommandHandler
{
    public async Task<int>  Handle(UpdateWordsCommand command, CancellationToken cancellationToken)
    {
        // Example logic to update words
        if (command.Words == null || command.Words.Count == 0)
        {
            return 0; // No words to update
        }

        var fileName = !command.Include ? "exclude.txt" : "include.txt";
        string filePath = Path.Combine(AppContext.BaseDirectory, "data", fileName);
        if (!File.Exists(filePath))
        {
            filePath = Path.Combine("resources", "definitions.txt");
        }

        // Read All lines
        var existingWords = File.ReadLines(filePath);
        
        // Determine which words need to be added
        var newWords = command.Words
            .Where(word => !existingWords.Contains(word, StringComparer.OrdinalIgnoreCase))
            .ToList();
        
        if(newWords.Count == 0)
            return 0;
        
        // Open the file for editing (reading and writing)
        using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            // // Read the current content
            // using (StreamReader reader = new StreamReader(fs))
            // {
            //     string currentContent = reader.ReadToEnd();
            //     Console.WriteLine("Current content:\n" + currentContent);
            // }
            // // Move the stream position to the end of the file to append new content
            fs.Seek(0, SeekOrigin.End);
            // Write new content
            using (StreamWriter writer = new StreamWriter(fs))
            {
                foreach (var word in newWords)
                {
                    writer.WriteLine(word.ToLower());
                }
            }
        }

        return newWords.Count;
    }
}