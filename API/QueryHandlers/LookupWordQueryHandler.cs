using service_matrix.DTO;
using service_matrix.Helpers;
using service_matrix.Queries;

namespace service_matrix.QueryHandlers;

public class LookupWordQueryHandler
{
    public async Task<List<LookupResultResponseItem>> Handle(LookupWordQuery query, CancellationToken cancellationToken)
    {
        var result = new List<LookupResultResponseItem>();

        try
        {
            if (query.Word == null || query.Word.Length < 4) throw new Exception("At least 4 chars required");
            var dictionaryList = FileHelper.ReadFileAsync("resources", "definitions.txt");
            var mergedList = FileHelper.ReadFileAsync("resources", "merged.txt");
            var includedList = FileHelper.ReadFileAsync("data", "include.txt");
            var excludedList = FileHelper.ReadFileAsync("data", "exclude.txt");

            var searchWord = query.Word.ToLower().Trim();
            foreach (var word in dictionaryList)
            {
                if (word.Contains(searchWord))
                {
                    result.Add(new LookupResultResponseItem(word, WordLocation.Dictionary));
                }
                
                if (result.Count() > 100)
                {
                    throw new Exception("Too many results, narrow your search");
                }
            }

            foreach (var word in mergedList)
            {
                if (word.Contains(searchWord))
                {
                    result.Add(new LookupResultResponseItem(word, WordLocation.Merged));
                }

                if (result.Count() > 100)
                {
                    throw new Exception("Too many results, narrow your search");
                }
            }

            foreach (var word in includedList)
            {
                if (word.Contains(searchWord))
                {
                    result.Add(new LookupResultResponseItem(word, WordLocation.Included));
                }
                
                if (result.Count() > 100)
                {
                    throw new Exception("Too many results, narrow your search");
                }
                
            }

            foreach (var word in excludedList)
            {
                if (word.Contains(searchWord))
                {
                    result.Add(new LookupResultResponseItem(word, WordLocation.Excluded));
                }
                
                if (result.Count() > 100)
                {
                    throw new Exception("Too many results, narrow your search");
                }
            }

            return result;
        }
        catch (Exception e)
        {
            result.Clear();
            result.Add(new LookupResultResponseItem(e.Message, WordLocation.Error));
            return result;
        }
    }
}