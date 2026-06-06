using Microsoft.Extensions.Logging;
using service_matrix.DTO;
using service_matrix.Helpers;
using service_matrix.Queries;

namespace service_matrix.QueryHandlers;

/// <summary>
/// Handles lookup word queries by searching across all dictionary sources.
/// </summary>
public class LookupWordQueryHandler
{
    private readonly DictionaryCacheService _dictionaryCache;
    private readonly ILogger<LookupWordQueryHandler> _logger;

         /// <summary>
         /// Initializes a new instance of the <see cref="LookupWordQueryHandler"/> class.
         /// </summary>
         /// <param name="dictionaryCache">The dictionary cache service.</param>
         /// <param name="logger">The logger.</param>
    public LookupWordQueryHandler(DictionaryCacheService dictionaryCache, ILogger<LookupWordQueryHandler> logger)
        {
            _dictionaryCache = dictionaryCache;
            _logger = logger;
         }

          /// <summary>
          /// Handles the lookup word query.
          /// </summary>
          /// <param name="query">The lookup word query containing the word and exact match flag.</param>
          /// <param name="cancellationToken">A cancellation token.</param>
          /// <returns>A list of lookup result response items matching the word.</returns>
    public Task<List<LookupResultResponseItem>> Handle(LookupWordQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Looking up word '{Word}' with ExactMatch={ExactMatch}", query.Word, query.ExactMatch);

           var results = new List<LookupResultResponseItem>();

           // Search across all cached dictionary sources
           foreach (var word in _dictionaryCache.Definitions.Concat(_dictionaryCache.Merged).Concat(_dictionaryCache.Include).Concat(_dictionaryCache.Exclude))
               {
               if (query.ExactMatch)
                   {
                   if (word.Equals(query.Word, StringComparison.OrdinalIgnoreCase))
                       {
                       results.Add(new LookupResultResponseItem(word, "Dictionary"));
                           _logger.LogDebug("Found exact match: {Word}", word);
                       }
                   }
               else
                   {
                   if (word.Contains(query.Word, StringComparison.OrdinalIgnoreCase))
                       {
                       results.Add(new LookupResultResponseItem(word, "Dictionary"));
                           _logger.LogDebug("Found partial match: {Word}", word);
                       }
                   }
               }

            _logger.LogInformation("Lookup completed. Found {Count} results for '{Word}'.", results.Count, query.Word);
           return Task.FromResult(results);
         }
}
