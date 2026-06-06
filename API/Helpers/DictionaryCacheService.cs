using Microsoft.Extensions.Logging;

namespace service_matrix.Helpers;

/// <summary>
/// Cache for dictionary files. Loads all dictionary sources once per request
/// and serves them from memory, eliminating per-request file I/O in handlers.
/// </summary>
public class DictionaryCacheService
{
    private readonly ILogger<DictionaryCacheService> _logger;
    private readonly HashSet<string> _definitions;
    private readonly HashSet<string> _merged;
    private readonly HashSet<string> _include;
    private readonly HashSet<string> _exclude;

     /// <summary>
     /// Initializes a new instance of the <see cref="DictionaryCacheService"/> class.
     /// Loads all dictionary files from disk once.
     /// </summary>
     /// <param name="fileHelper">The file helper service for reading initial data.</param>
     /// <param name="logger">The logger.</param>
    public DictionaryCacheService(IFileHelper fileHelper, ILogger<DictionaryCacheService> logger)
     {
         _logger = logger;

        var definitions = fileHelper.ReadFile("resources", "definitions.txt");
        var merged = fileHelper.ReadFile("resources", "merged.txt");
        var includeList = fileHelper.ReadFile("data", "include.txt");
        var excludeList = fileHelper.ReadFile("data", "exclude.txt");

         _definitions = new HashSet<string>(definitions, StringComparer.OrdinalIgnoreCase);
         _merged = new HashSet<string>(merged, StringComparer.OrdinalIgnoreCase);
         _include = new HashSet<string>(includeList, StringComparer.OrdinalIgnoreCase);
         _exclude = new HashSet<string>(excludeList, StringComparer.OrdinalIgnoreCase);

         _logger.LogInformation(
             "Dictionary cache loaded: {Definitions} definitions, {Merged} merged, {Include} included, {Exclude} excluded.",
             _definitions.Count, _merged.Count, _include.Count, _exclude.Count);
     }

     /// <summary>
     /// Gets all definition words (from definitions.txt and merged.txt combined).
     /// </summary>
    public IEnumerable<string> Definitions => _definitions;

     /// <summary>
     /// Gets merged words.
     /// </summary>
    public IEnumerable<string> Merged => _merged;

     /// <summary>
     /// Gets included words.
     /// </summary>
    public IEnumerable<string> Include => _include;

     /// <summary>
     /// Gets excluded words.
     /// </summary>
    public IEnumerable<string> Exclude => _exclude;

     /// <summary>
     /// Gets the combined dictionary (definitions + merged - exclude), deduplicated.
     /// </summary>
     /// <param name="minLength">Minimum word length filter.</param>
     /// <param name="maxLength">Maximum word length filter.</param>
     /// <returns>An enumerable of unique words matching the length constraints.</returns>
    public IEnumerable<string> GetFilteredDictionary(int minLength, int maxLength)
        {
        var excludeSet = _exclude;
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var word in _definitions.Concat(_merged))
            {
            if (word.Length >= minLength && word.Length <= maxLength && !excludeSet.Contains(word) && seen.Add(word))
                yield return word;
            }

            // Include words that aren't already in definitions/merged and not excluded
        foreach (var word in _include)
            {
            if (!excludeSet.Contains(word) && word.Length >= minLength && seen.Add(word))
                yield return word;
            }
        }

     /// <summary>
     /// Refreshes the cache by re-reading all dictionary files from disk.
     /// Useful after merge or update operations.
     /// </summary>
    public void Refresh(IFileHelper fileHelper)
     {
        var definitions = fileHelper.ReadFile("resources", "definitions.txt");
        var merged = fileHelper.ReadFile("resources", "merged.txt");
        var includeList = fileHelper.ReadFile("data", "include.txt");
        var excludeList = fileHelper.ReadFile("data", "exclude.txt");

         _definitions.Clear();
        foreach (var w in definitions) _definitions.Add(w);
         _merged.Clear();
        foreach (var w in merged) _merged.Add(w);
         _include.Clear();
        foreach (var w in includeList) _include.Add(w);
         _exclude.Clear();
        foreach (var w in excludeList) _exclude.Add(w);

         _logger.LogInformation(
             "Dictionary cache refreshed: {Definitions} definitions, {Merged} merged, {Include} included, {Exclude} excluded.",
             _definitions.Count, _merged.Count, _include.Count, _exclude.Count);
     }
}
