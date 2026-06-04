using service_matrix.Commands;
using service_matrix.DTO;
using service_matrix.Helpers;

namespace service_matrix.CommandHandlers;

/// <summary>
/// Handles merge words commands using injected file service.
/// </summary>
public class MergeWordsCommandHandler
{
    private readonly IFileHelper _fileHelper;

        /// <summary>
        /// Constructor with dependency injection.
        /// </summary>
        /// <param name="fileHelper">The file helper service.</param>
    public MergeWordsCommandHandler(IFileHelper fileHelper)
        {
           _fileHelper = fileHelper;
        }

        /// <summary>
         /// 
         /// </summary>
         /// <param name="cmd"></param>
         /// <param name="cancellationToken"></param>
         /// <returns></returns>
    public async Task<MergeResponse> Handle(MergeWordsCommand cmd, CancellationToken cancellationToken)
        {
        var removedCounter = 0;
        
        var includes = _fileHelper.ReadFile("data", "include.txt").ToList();
          // var excludes = FileHelper.ReadFileAsync("data", "exclude.txt").ToList();
        var dictionary = _fileHelper.ReadFile("resources", "definitions.txt").ToHashSet();
        var merged = _fileHelper.ReadFile("resources", "merged.txt");
        dictionary.UnionWith(merged);
        
        var mergedList = new List<string>();
        foreach (var include in includes)
            {
            if (include.Length < 4 || include.IndexOf('-') > -1 || include.IndexOf(' ') > -1)
                {
                continue;
                }

            var includeFormatted = include.ToLower().Trim().Replace('ё', 'е');
            if (!dictionary.Contains(includeFormatted))
                {
                mergedList.Add(includeFormatted);
                }
            }
        includes = includes.Except(mergedList).ToList();
        
          //Save all files
        await _fileHelper.WriteFileNewContents("data", "mergeable_definitions.txt", mergedList);
        await _fileHelper.WriteFileNewContents("data", "include.txt", includes);
        
        var res = new MergeResponse(mergedList.Count, removedCounter);
        return res;
        }
    
}