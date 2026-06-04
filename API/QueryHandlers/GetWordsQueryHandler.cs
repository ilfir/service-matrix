using Microsoft.Extensions.Logging;
using service_matrix.Helpers;
using service_matrix.Queries;

namespace service_matrix.QueryHandlers;

/// <summary>
/// Handles get words queries by reading from the include or exclude list files.
/// </summary>
public class GetWordsQueryHandler
{
    private readonly IFileHelper _fileHelper;
    private readonly ILogger<GetWordsQueryHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetWordsQueryHandler"/> class.
        /// </summary>
        /// <param name="fileHelper">The file helper service.</param>
        /// <param name="logger">The logger.</param>
    public GetWordsQueryHandler(IFileHelper fileHelper, ILogger<GetWordsQueryHandler> logger)
        {
           _fileHelper = fileHelper;
           _logger = logger;
        }

         /// <summary>
         /// Handles the get words query.
         /// </summary>
         /// <param name="query">The get words query containing the include flag.</param>
         /// <param name="cancellationToken">A cancellation token.</param>
         /// <returns>A list of words from the specified file.</returns>
    public Task<IEnumerable<string>> Handle(GetWordsQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting words with Include={Include}", query.Include);

             string fileName = query.Include ? "include.txt" : "exclude.txt";
             var words = _fileHelper.ReadFile("data", fileName);

              _logger.LogInformation("Retrieved {WordCount} words from {FileName}.", words.Count(), fileName);
             return Task.FromResult(words);
        }
}