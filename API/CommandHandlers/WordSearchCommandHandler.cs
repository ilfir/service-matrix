using service_matrix.Helpers;
using service_matrix.Commands;

namespace service_matrix.CommandHandlers;

/// <summary>
/// Handles word search commands using injected file and search services.
/// </summary>
public class WordSearchCommandHandler
{
    private readonly IFileHelper _fileHelper;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    /// <param name="fileHelper">The file helper service.</param>
    public WordSearchCommandHandler(IFileHelper fileHelper)
    {
        _fileHelper = fileHelper;
    }

     /// <summary>
      /// 
      /// </summary>
      /// <param name="command"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
    public Task<Dictionary<string, Dictionary<int, Dictionary<string, string>>>> Handle(WordSearchCommand command, CancellationToken cancellationToken)
     {
        var definitionWords = new HashSet<string>();
        var dictionary = _fileHelper.ReadFile("resources", "definitions.txt");
        var mergedDictionary = _fileHelper.ReadFile("resources", "merged.txt");
        
        foreach (string line in dictionary.Concat(mergedDictionary))
          {
            if(line.Length > command.MaxLength || line.Length < command.MinLength)
              {
                continue;
              }
            definitionWords.Add(line);
          }
        
        var includes = _fileHelper.ReadFile("data", "include.txt");
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
        var excludes = _fileHelper.ReadFile("data", "exclude.txt");
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

        return Task.FromResult(topResults);
     }
}