using service_matrix.DTO;
using service_matrix.Helpers;
using service_matrix.Queries;

namespace service_matrix.QueryHandlers;

/// <summary>
/// 
/// </summary>
public class LookupWordQueryHandler
{
    private FileHelper _helper;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="helper"></param>
    public LookupWordQueryHandler(FileHelper helper)
    {
        _helper = helper;
    }

    /// <summary>
    /// Handler
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
            
            result.AddRange(await FindWordInDictionary("DefinitionsFilePath", query, WordLocation.Dictionary));
            result.AddRange(await FindWordInDictionary("MergedFilePath", query, WordLocation.Merged));
            result.AddRange(await FindWordInDictionary("IncludeFilePath", query, WordLocation.Included));
            result.AddRange(await FindWordInDictionary("ExcludeFilePath", query, WordLocation.Excluded));
            
            return await Task.FromResult(result);
        }
        catch (Exception e)
        {
            result.Clear();
            result.Add(new LookupResultResponseItem(e.Message, WordLocation.Error.ToString()));
            return await Task.FromResult(result);
        }
    }

    private async Task<IEnumerable<LookupResultResponseItem>> FindWordInDictionary(string pathKeyOrPath, LookupWordQuery query, WordLocation loc)
    {
        var list = new List<LookupResultResponseItem>();
        var dict = await _helper.ReadFileAsync(pathKeyOrPath);
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