using System.Diagnostics;
using service_matrix.Helpers;

public class WordSearchCommandHandler
{
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
        // definitionWords = definitionWords.OrderByDescending(word => word.Length).ToList();

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

        var foundWordsList = new Dictionary<string, Dictionary<int, Dictionary<string, string>>>();
        foreach (var definitionWord in definitionWords)
        {
            // if (foundWordsList.Count >= command.MaxWords)
            // {
            //     break;
            // }

            if (foundWordsList.Keys.Contains(definitionWord))
            {
                continue;
            }

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
                    foundWordsList.Add(foundWord, searchHelper.GetFoundWord());
                }

            }
        }
        
        // foundWordsList = foundWordsList.OrderByDescending(entry => entry.Key.Length).ToDictionary(entry => entry.Key, entry => entry.Value);
        var top20 = foundWordsList
            .OrderByDescending(pair => pair.Key.Length)
            .Take(command.MaxWords) 
            .ToDictionary(pair => pair.Key, pair => pair.Value);

        return Task.FromResult(foundWordsList);
    }
}