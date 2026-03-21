using service_matrix.Commands;
using service_matrix.DTO;
using service_matrix.Helpers;

namespace service_matrix.CommandHandlers;

/// <summary>
/// Merge words processing
/// </summary>
public class MergeWordsCommandHandler
{
    private readonly FileHelper _fileHelper;

    /// <summary>
    /// MergeWordsCommandHandler
    /// </summary>
    /// <param name="fileHelper"></param>
    public MergeWordsCommandHandler(FileHelper fileHelper)
    {
        _fileHelper = fileHelper;
    }

    /// <summary>
    /// Handle merge command
    /// </summary>
    /// <param name="cmd">The merge command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Merge response with counts</returns>
    public async Task<MergeResponse> Handle(MergeWordsCommand cmd, CancellationToken cancellationToken)
    {
        var removedCounter = 0;
        
        var includes = (await _fileHelper.ReadFileAsync("IncludeFilePath")).ToList();
        // var excludes = FileHelper.ReadFileAsync("data", "exclude.txt").ToList();
        var dictionary = (await _fileHelper.ReadFileAsync("DefinitionsFilePath")).ToHashSet();
        var merged = await _fileHelper.ReadFileAsync("MergedFilePath");
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
            else
            {
                removedCounter++; // IMP-010: Track words that were already in dictionary
            }
        }
        includes = includes.Except(mergedList).ToList();
        
        //Save all files
        await _fileHelper.WriteFileNewContents(mergedList, "data/mergeable_definitions.txt");
        await _fileHelper.WriteFileNewContents(includes, "IncludeFilePath");
        
        var res = new MergeResponse(mergedList.Count, removedCounter);
        return res;
    }
    
}