using Microsoft.Extensions.Logging;
using service_matrix.Helpers;
using service_matrix.Commands;

namespace service_matrix.CommandHandlers;

/// <summary>
/// Handles word search commands using injected file and search services.
/// </summary>
public class WordSearchCommandHandler
{
    private readonly DictionaryCacheService _dictionaryCache;
    private readonly ILogger<WordSearchCommandHandler> _logger;

       /// <summary>
       /// Initializes a new instance of the <see cref="WordSearchCommandHandler"/> class.
       /// </summary>
       /// <param name="dictionaryCache">The dictionary cache service.</param>
       /// <param name="logger">The logger.</param>
    public WordSearchCommandHandler(DictionaryCacheService dictionaryCache, ILogger<WordSearchCommandHandler> logger)
       {
           _dictionaryCache = dictionaryCache;
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

          // Precompute the set of all available letters in the matrix once.
          // This replaces IsAllLettersInMatrix's O(matrix_size^2) per-word check
          // with an O(word_length) HashSet lookup after one-time setup.
        var availableLetters = new HashSet<char>();
        for (int i = 0; i < rows; i++)
           {
            for (int j = 0; j < columns; j++)
               {
                var cell = lettersMatrix2D[i, j];
                if (!string.IsNullOrEmpty(cell))
                    availableLetters.Add(cell[0]);
               }
           }

          _logger.LogDebug("Precomputed {LetterCount} unique letters in matrix.", availableLetters.Count);

        var foundWordsList = new Dictionary<string, Dictionary<int, Dictionary<string, string>>>();
        int checkedCount = 0;

          // Use cached dictionary instead of reading files per request
        foreach (var definitionWord in _dictionaryCache.GetFilteredDictionary(command.MinLength, command.MaxLength))
          {
            checkedCount++;

              // Fast pre-filter: check if all letters exist in the matrix.
              // Uses the precomputed HashSet — O(word_length) instead of O(matrix_size^2).
            bool allLettersPresent = true;
            foreach (char c in definitionWord)
              {
                if (!availableLetters.Contains(c))
                  {
                    allLettersPresent = false;
                    break;
                  }
              }

            if (!allLettersPresent)
                continue;

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
