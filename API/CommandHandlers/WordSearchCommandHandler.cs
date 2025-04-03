using service_matrix.Helpers;

namespace service_matrix.CommandHandlers;

/// <summary>
/// 
/// </summary>
public class WordSearchCommandHandler
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Dictionary<string, Dictionary<int, Dictionary<string, string>>>> Handle(WordSearchCommand command, CancellationToken cancellationToken)
    {
        List<string> definitionWords = new List<string>();
        var dictionary = FileHelper.ReadFileAsync( "resources", "definitions.txt").ToList();
        var mergedDictionary = FileHelper.ReadFileAsync( "resources", "merged.txt").ToList();
        dictionary.AddRange(mergedDictionary);
        foreach (string line in dictionary)
        {
            if(line.Length > command.MaxLength || line.Length < command.MinLength)
            {
                continue;
            }
            definitionWords.Add(line);
        }
       
        var includes = FileHelper.ReadFileAsync( "data", "include.txt");
        foreach (string line in includes)
        {
            if (line.Length > 25)
            {
                continue;
            }

            if(line.Length < command.MinLength)
            {
                break;
            }
            definitionWords.Add(line);
        }
        var excludes = FileHelper.ReadFileAsync( "data", "exclude.txt");
        foreach (string line in excludes)
        {
            if(line.Length > command.MaxLength || line.Length < command.MinLength)
            {
                continue;
            }
            definitionWords.Remove(line);
        }

        int rows = command.LettersMatrix.Count;
        int columns = command.LettersMatrix[0].Count;
        string[,] lettersMatrix2D = new string[rows, columns];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                lettersMatrix2D[i, j] = command.LettersMatrix[i][j];
            }
        }

        var foundWordsList = new Dictionary<string, Dictionary<int, Dictionary<string, string>>>();
        foreach (var definitionWord in definitionWords)
        {
            if (foundWordsList.Keys.Contains(definitionWord) || !WordSearchHelper.IsAllLettersInMatrix(lettersMatrix2D, definitionWord))
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
                    foundWordsList.Add(foundWord, searchHelper.GetFoundWord());
                }

            }
        }
        
        var topResults = foundWordsList
            .OrderByDescending(pair => pair.Key.Length)
            .Take(command.MaxWords) 
            .ToDictionary(pair => pair.Key, pair => pair.Value);

        return Task.FromResult(topResults);
    }
}