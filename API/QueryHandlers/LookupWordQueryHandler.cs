using service_matrix.DTO;
using service_matrix.Helpers;
using service_matrix.Queries;

namespace service_matrix.QueryHandlers;

/// <summary>
/// 
/// </summary>
public class LookupWordQueryHandler
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<List<LookupResultResponseItem>> Handle(LookupWordQuery query, CancellationToken cancellationToken)
    {
        var result = new List<LookupResultResponseItem>();

        try
        {
            if (query.Word == null || query.Word.Length < 4) throw new Exception("At least 4 chars required");
            
            result.AddRange(FindWordInDictionary("resources", "definitions.txt", query, WordLocation.Dictionary));
            result.AddRange(FindWordInDictionary("resources", "merged.txt", query, WordLocation.Merged));
            result.AddRange(FindWordInDictionary("data", "include.txt", query, WordLocation.Included));
            result.AddRange(FindWordInDictionary("data", "exclude.txt", query, WordLocation.Excluded));
            
            return result;
        }
        catch (Exception e)
        {
            result.Clear();
            result.Add(new LookupResultResponseItem(e.Message, WordLocation.Error.ToString()));
            return result;
        }
    }

    private IEnumerable<LookupResultResponseItem> FindWordInDictionary(string dir, string file, LookupWordQuery query, WordLocation loc)
    {
        var list = new List<LookupResultResponseItem>();
        var dict = FileHelper.ReadFileAsync(dir, file);
        var searchWord = query.Word.ToLower().Trim();
        foreach (var word in dict)
        {
            if ((!query.ExactMatch && word.Contains(searchWord)) || string.Equals(searchWord, word))
            {
                list.Add(new LookupResultResponseItem(word, loc.ToString()));
            }
            
            if (list.Count() > 100)
            {
                throw new Exception("Too many results, narrow your search");
            }
        }

        return list;
    }
}