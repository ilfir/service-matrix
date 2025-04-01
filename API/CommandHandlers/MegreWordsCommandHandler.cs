using service_matrix.DTO;
using service_matrix.Helpers;

namespace service_matrix.CommandHandlers;

public class MergeWordsCommandHandler
{
    public async Task<MergeResponse> Handle(CancellationToken cancellationToken)
    {
        var addedCounter = 0;
        var removedCounter = 0;
        
        var includes = FileHelper.ReadFileAsync("data", "include.txt");
        var excludes = FileHelper.ReadFileAsync("data", "exclude.txt");
        var dictionary = FileHelper.ReadFileAsync( "resources", "definitions.txt");

        var includedList = new List<string>();
        foreach (var include in includes)
        {
            var includeFormatted = include.ToLower().Trim();
            if (!dictionary.Contains(include))
            {
                dictionary.ToList().Add(includeFormatted);
                addedCounter++;
                includedList.Add(include);
            }
        }
        includes = includes.Except(includedList);

        var excludedList = new List<string>();
        foreach (var exclude in excludes)
        {
            var excludeFormatted = exclude.ToLower().Trim();
            if (dictionary.Contains(exclude))
            {
                dictionary.ToList().Remove(excludeFormatted);
                removedCounter++;
                excludedList.Add(exclude);
            }
        }
        excludes = excludes.Except(excludedList);
        
        //Save all files
        await FileHelper.WriteFileNewContents(dictionary, "resources", "definitions.txt");
        await FileHelper.WriteFileNewContents(includes, "data", "include.txt");
        await FileHelper.WriteFileNewContents(excludes, "data", "exclude.txt");
        
        var res = new MergeResponse(addedCounter, removedCounter);
        return res;
    }
    
    
    
}