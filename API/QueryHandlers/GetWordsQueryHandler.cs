using service_matrix.Helpers;
using service_matrix.Queries;

namespace service_matrix.QueryHandlers;

/// <summary>
/// Handles get words queries using injected file service.
/// </summary>
public class GetWordsQueryHandler
{
    private readonly IFileHelper _fileHelper;

       /// <summary>
       /// Constructor with dependency injection.
       /// </summary>
       /// <param name="fileHelper">The file helper service.</param>
    public GetWordsQueryHandler(IFileHelper fileHelper)
       {
           _fileHelper = fileHelper;
       }

       /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
    public async Task<List<string>> Handle(GetWordsQuery query, CancellationToken cancellationToken)
       {
           var fileName = !query.Include ? "exclude.txt" : "include.txt";
           var words = _fileHelper.ReadFile("data", fileName);

           return new List<string>(words);
       }
}