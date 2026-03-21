using service_matrix.Helpers;

namespace service_matrix.CommandHandlers;

/// <summary>
/// 
/// </summary>
public class WordSearchCommandHandler
{
    private readonly FileHelper _fileHelper;

    /// <summary>
    /// WordSearchCommandHandler
    /// </summary>
    /// <param name="fileHelper"></param>
    public WordSearchCommandHandler(FileHelper fileHelper)
    {
        _fileHelper = fileHelper;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Dictionary<string, Dictionary<int, Dictionary<string, string>>>> Handle(WordSearchCommand command, CancellationToken cancellationToken)
    {
        var definitionWords = new HashSet<string>();
        var dictionary = await _fileHelper.ReadFileAsync( "DefinitionsFilePath");
        var mergedDictionary = await _fileHelper.ReadFileAsync( "MergedFilePath");
        
        foreach (string line in dictionary.Concat(mergedDictionary))
        {
            if(line.Length > command.MaxLength || line.Length < command.MinLength)
            {
                continue;
            }
            definitionWords.Add(line);
        }
       
        var includes = await _fileHelper.ReadFileAsync( "IncludeFilePath");
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
        var excludes = await _fileHelper.ReadFileAsync( "ExcludeFilePath");
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
            if (foundWordsList.ContainsKey(definitionWord) || !WordSearchHelper.IsAllLettersInMatrix(lettersMatrix2D, definitionWord))
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

        return await Task.FromResult(topResults);
    }
}