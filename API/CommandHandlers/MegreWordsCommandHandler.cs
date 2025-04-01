using service_matrix.Commands;
using service_matrix.DTO;
using service_matrix.Helpers;

namespace service_matrix.CommandHandlers;

public class MergeWordsCommandHandler
{
    public async Task<MergeResponse> Handle(MergeWordsCommand cmd, CancellationToken cancellationToken)
    {
        // var addedCounter = 0;
        var removedCounter = 0;
        
        var includes = FileHelper.ReadFileAsync("data", "include.txt").ToList();
        // var excludes = FileHelper.ReadFileAsync("data", "exclude.txt").ToList();
        var dictionary = FileHelper.ReadFileAsync( "resources", "definitions.txt").ToList();
        var merged = FileHelper.ReadFileAsync( "resources", "merged.txt").ToList();
        dictionary.AddRange(merged);
        
        var mergedList = new List<string>();
        foreach (var include in includes)
        {
            if (include.Length < 5 || include.IndexOf('-') > -1 || include.IndexOf(' ') > -1)
            {
                continue;
            }

            var includeFormatted = include.ToLower().Trim().Replace('ё', 'е');
            if (!dictionary.Contains(includeFormatted))
            {
                mergedList.Add(includeFormatted);
                // dictionary.Add(includeFormatted);
                // addedCounter++;
            }
        }
        includes = includes.Except(mergedList).ToList();

        // var excludedList = new List<string>();
        // foreach (var exclude in excludes)
        // {
        //     var excludeFormatted = exclude.ToLower().Trim();
        //     var isRemoved = dictionary.Remove(excludeFormatted);
        //     if(isRemoved)
        //     {
        //         removedCounter++;
        //         excludedList.Add(exclude);
        //     }
        //
        // }
        // excludes = excludes.Except(excludedList).ToList();
        
        //Save all files
        await FileHelper.WriteFileNewContents(mergedList, "data", "mergeable_definitions.txt");
        await FileHelper.WriteFileNewContents(includes, "data", "include.txt");
        // await FileHelper.WriteFileNewContents(excludes, "data", "exclude.txt");
        
        var res = new MergeResponse(mergedList.Count, removedCounter);
        return res;
    }
    
}