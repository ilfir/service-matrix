using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public class WordSearchCommandHandler
{
    public Task<string> Handle(WordSearchCommand command, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();


        List<string> definitionWords = new List<string>();
        string filePath = Path.Combine(AppContext.BaseDirectory, "resources", "definitions.txt");
        foreach (string line in File.ReadLines(filePath))
        {
            definitionWords.Add(line);
        }

        stopwatch.Stop();
        double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
        var resp = $"Time taken: {elapsedSeconds} seconds, read {definitionWords.Count} words";
        // System.Console.WriteLine($"Time taken: {elapsedSeconds} seconds");
// return resp;


        // Implement your word search logic here
        List<string> foundWords = new List<string>();

        // foundWords.Add(resp);

        // // Example logic (replace with actual implementation)
        // foreach (var row in command.lettersMatrix)
        // {
        //     foreach (var letter in row)
        //     {
        //         if (letter.Length >= command.MinLength && letter.Length <= command.MaxLength)
        //         {
        //             foundWords.Add(letter);
        //         }
        //     }
        // }

        return Task.FromResult(resp);
    }
}