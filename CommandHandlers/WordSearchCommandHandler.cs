using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using service_matrix.Helpers;

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
            if(line.Length > command.MaxLength || line.Length < command.MinLength)
            {
                continue;
            }
            definitionWords.Add(line);
        }
        definitionWords.Add("test");

        stopwatch.Stop();
        double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
        var resp = $"Time taken: {elapsedSeconds} seconds, read {definitionWords.Count} words";

        int rows = command.lettersMatrix.Count;
        int columns = command.lettersMatrix[0].Count;
        string[,] lettersMatrix2D = new string[rows, columns];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                lettersMatrix2D[i, j] = command.lettersMatrix[i][j];
            }
        }
        
        var foundWordsString = new StringBuilder("Found words: ");
        foreach (var definitionWord in definitionWords)
        {
            var searchHelper = new WordSearchHelper(definitionWord, lettersMatrix2D);
            if(searchHelper.IsAllLettersInMatrix(lettersMatrix2D, definitionWord) == false)
            {
                continue;
            }
            var searchResult = searchHelper.Search();
            if (searchResult == true)
            {
                var foundWord = searchHelper.GetFoundString();

                if (string.Equals(definitionWord, foundWord, StringComparison.OrdinalIgnoreCase))
                {
                    foundWordsString.Append(foundWord);
                    foundWordsString.Append(", Coordinates: ");

                    foreach (var wordEntry in searchHelper.GetFoundWord())
                    {
                        foreach (var letterEntry in wordEntry.Value)
                        {
                            foundWordsString.Append($"{letterEntry.Key} at {letterEntry.Value}, ");
                        }
                    }
                    foundWordsString.Append(" | ");
                }

                resp = foundWordsString.ToString().TrimEnd(',', ' ');
                // break;
            }
        }



        return Task.FromResult(resp);
    }
}