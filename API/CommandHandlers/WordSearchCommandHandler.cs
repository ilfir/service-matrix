using service_matrix.Helpers;

namespace service_matrix.CommandHandlers;

/// <summary>
/// Command handler
/// </summary>
public class WordSearchCommandHandler
{
    /// <summary>
    /// Handler
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Dictionary<string, Dictionary<int, Dictionary<string, string>>>> HandleAsync(WordSearchCommand cmd, CancellationToken cancellationToken)
    {
        List<string> wordList = new List<string>();

        // Read files using helper function
        wordList = await ReadFilesAsync(cmd, wordList);

        // Matrix creation
        int rows = cmd.LettersMatrix.Count;
        int columns = cmd.LettersMatrix[0].Count;
        string[,] lettersMatrix2D = new string[rows, columns];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                lettersMatrix2D[i, j] = cmd.LettersMatrix[i][j];
            }
        }

        // Search logic
        var results = new Dictionary<string, Dictionary<int, Dictionary<string, string>>>();
        foreach (string definitionWord in wordList)
        {
            if (results.ContainsKey(definitionWord) || !WordSearchHelper.IsAllLettersInMatrix(lettersMatrix2D, definitionWord))
            {
                continue;
            }

            var searchHelper = new WordSearchHelper(definitionWord, lettersMatrix2D);
            var searchResult = searchHelper.Search();
            if (searchResult == true)
            {
                var foundWord = searchHelper.GetFoundString();

                if (string.Equals(definitionWord, foundWord, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(foundWord, searchHelper.GetFoundWord());
                }
            }
        }

        //Order by length, limit to maxWords
        results = results
            .OrderByDescending(pair => pair.Key.Length)
            .Take(cmd.MaxWords)
            .ToDictionary(pair => pair.Key, pair => pair.Value);

        return await Task.FromResult(results);
    }
    private async Task<List<string>> ReadFilesAsync(WordSearchCommand cmd, List<string> wordList)
    {
        var definitionWords = new List<string>();
        var inclusionList = FileHelper.ReadFileAsync("resources", "inclusion.txt");
        var exclusionList = FileHelper.ReadFileAsync("data", "exclusion.txt");

        foreach (string line in inclusionList)
        {
            if (line.Length > 25)
            {
                continue;
            }
            wordList.Add(line);
        }

        foreach (string line in exclusionList)
        {
            if (line.Length > 25)
            {
                continue;
            }
            wordList.Remove(line);
        }

        return wordList;
    }
}
