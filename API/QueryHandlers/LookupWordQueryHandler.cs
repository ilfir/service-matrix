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
    private readonly IFileHelper _fileHelper;
    private readonly ILogger<LookupWordQueryHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LookupWordQueryHandler"/> class.
        /// </summary>
        /// <param name="fileHelper">The file helper service.</param>
        /// <param name="logger">The logger.</param>
    public LookupWordQueryHandler(IFileHelper fileHelper, ILogger<LookupWordQueryHandler> logger)
       {
           _fileHelper = fileHelper;
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

           var definitions = _fileHelper.ReadFile("resources", "definitions.txt");
           var merged = _fileHelper.ReadFile("resources", "merged.txt");
           var includeList = _fileHelper.ReadFile("data", "include.txt");
           var excludeList = _fileHelper.ReadFile("data", "exclude.txt");

           var results = new List<LookupResultResponseItem>();

           foreach (var word in definitions.Concat(merged).Concat(includeList).Concat(excludeList))
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