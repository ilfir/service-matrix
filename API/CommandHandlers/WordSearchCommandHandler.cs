using Microsoft.Extensions.Logging;
using service_matrix.Helpers;
using service_matrix.Commands;

namespace service_matrix.CommandHandlers;

/// <summary>
/// Handles word search commands using injected file and search services.
/// </summary>
public class WordSearchCommandHandler
{
    private readonly IFileHelper _fileHelper;
    private readonly ILogger<WordSearchCommandHandler> _logger;

     /// <summary>
     /// Initializes a new instance of the <see cref="WordSearchCommandHandler"/> class.
     /// </summary>
     /// <param name="fileHelper">The file helper service.</param>
     /// <param name="logger">The logger.</param>
    public WordSearchCommandHandler(IFileHelper fileHelper, ILogger<WordSearchCommandHandler> logger)
     {
         _fileHelper = fileHelper;
         _logger = logger;
     }

       /// <summary>
       /// Handles the word search command.
       /// </summary>
       /// <param name="command">The word search command containing matrix and parameters.</param>
       /// <param name="cancellationToken">A cancellation token.</param>
       /// <returns>A dictionary of found words with their locations in the matrix.</returns>
    public async Task<Dictionary<string, Dictionary<int, Dictionary<string, string>>>> Handle(WordSearchCommand command, CancellationToken cancellationToken)
     {
         _logger.LogInformation("Starting word search with MinLength={MinLength}, MaxLength={MaxLength}, MaxWords={MaxWords}",
             command.MinLength, command.MaxLength, command.MaxWords);

         var definitionWords = new HashSet<string>();
         
         _logger.LogDebug("Reading definitions.txt");
         var dictionary = await _fileHelper.ReadFileAsync("resources", "definitions.txt");
         _logger.LogDebug("Reading merged.txt");
         var mergedDictionary = await _fileHelper.ReadFileAsync("resources", "merged.txt");
         
         foreach (string line in dictionary.Concat(mergedDictionary))
            {
             if(line.Length > command.MaxLength || line.Length < command.MinLength)
                {
                 continue;
                }
             definitionWords.Add(line);
            }
        
         _logger.LogDebug("Reading include.txt");
         var includes = await _fileHelper.ReadFileAsync("data", "include.txt");
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
        
         _logger.LogDebug("Reading exclude.txt");
         var excludes = await _fileHelper.ReadFileAsync("data", "exclude.txt");
         foreach (string line in excludes)
            {
             if(line.Length > command.MaxLength || line.Length < command.MinLength)
                {
                 continue;
                }
             definitionWords.Remove(line);
            }

         _logger.LogDebug("Loaded {DefinitionCount} definitions after filtering by length.", definitionWords.Count);

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
         int checkedCount = 0;
         
         foreach (var definitionWord in definitionWords)
           {
             checkedCount++;
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
                    _logger.LogDebug("Found word: {Word}", definitionWord);
                   }

                }
            }
        
         _logger.LogDebug("Checked {CheckedCount} words, found {FoundCount} words.", checkedCount, foundWordsList.Count);

         var topResults = foundWordsList
            .OrderByDescending(pair => pair.Key.Length)
            .Take(command.MaxWords) 
            .ToDictionary(pair => pair.Key, pair => pair.Value);

         _logger.LogInformation("Word search completed. Found {FoundCount} words out of {CheckedCount} checked.", topResults.Count, checkedCount);
         
         return topResults;
     }
}
