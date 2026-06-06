using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace service_matrix.Helpers;

/// <summary>
/// Cache for dictionary files. Loads all dictionary files once per application lifetime,
/// eliminating per-request file I/O in handlers.
/// </summary>
public class DictionaryCacheService
{
    private readonly ILogger<DictionaryCacheService> _logger;
    private HashSet<string> _definitions;
    private HashSet<string> _merged;
    private HashSet<string> _include;
    private HashSet<string> _exclude;

       /// <summary>
       /// Initializes a new instance of the <see cref="DictionaryCacheService"/> class.
       /// Loads all dictionary files from disk at startup (singleton lifetime).
       /// </summary>
       /// <param name="logger">The logger.</param>
    public DictionaryCacheService(ILogger<DictionaryCacheService> logger)
       {
          _logger = logger;
        LoadDictionariesFromDisk();
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
         /// Gets dictionary words filtered by minimum and maximum length.
         /// Excludes words in the exclude list and deduplicates across sources.
         /// </summary>
         /// <param name="minLength">The minimum word length (inclusive).</param>
         /// <param name="maxLength">The maximum word length (inclusive).</param>
         /// <returns>An enumerable of words whose length falls within the specified range.</returns>
       public IEnumerable<string> GetFilteredDictionary(int minLength, int maxLength)
           {
            var excludeSet = _exclude;
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var word in _definitions.Concat(_merged))
                 {
                if (word.Length >= minLength && word.Length <= maxLength && !excludeSet.Contains(word) && seen.Add(word))
                    yield return word;
                 }
           }

         /// <summary>
        /// Refreshes the cache by re-reading all dictionary files from disk.
        /// </summary>
      public void Refresh()
         {
            _logger.LogInformation("Refreshing dictionary cache from disk files...");
          LoadDictionariesFromDisk();
         }

         /// <summary>
         /// Loads all dictionary files from disk into memory.
         /// </summary>
         /// <param name="fileHelper">The file helper service for reading initial data.</param>
        private void LoadDictionaries(IFileHelper fileHelper)
      {
         try
           {
            var definitions = fileHelper.ReadFile("resources", "definitions.txt");
            var merged = fileHelper.ReadFile("resources", "merged.txt");
            var includeList = fileHelper.ReadFile("data", "include.txt");
            var excludeList = fileHelper.ReadFile("data", "exclude.txt");

              _definitions.Clear();
            foreach (var word in definitions) _definitions.Add(word);

              _merged.Clear();
            foreach (var word in merged) _merged.Add(word);

              _include.Clear();
            foreach (var word in includeList) _include.Add(word);

              _exclude.Clear();
            foreach (var word in excludeList) _exclude.Add(word);

              _logger.LogInformation(
                  "Dictionary cache loaded: {Definitions} definitions, {Merged} merged, {Include} included, {Exclude} excluded.",
                  _definitions.Count, _merged.Count, _include.Count, _exclude.Count);
           }
         catch (Exception ex)
           {
              _logger.LogError(ex, "Failed to load dictionary files");
            throw;
           }
        }

        /// <summary>
        /// Loads all dictionary files directly from disk (for singleton initialization).
        /// </summary>
       private void LoadDictionariesFromDisk()
       {
        try
          {
            string basePath = Path.Combine(AppContext.BaseDirectory, "resources");
            string dataPath = Path.Combine(AppContext.BaseDirectory, "data");

            var definitions = File.ReadAllLines(Path.Combine(basePath, "definitions.txt")).ToList();
            var merged = File.ReadAllLines(Path.Combine(basePath, "merged.txt")).ToList();
            var includeList = File.ReadAllLines(Path.Combine(dataPath, "include.txt")).ToList();
            var excludeList = File.ReadAllLines(Path.Combine(dataPath, "exclude.txt")).ToList();

            _definitions = new HashSet<string>(definitions, StringComparer.OrdinalIgnoreCase);
            _merged = new HashSet<string>(merged, StringComparer.OrdinalIgnoreCase);
            _include = new HashSet<string>(includeList, StringComparer.OrdinalIgnoreCase);
            _exclude = new HashSet<string>(excludeList, StringComparer.OrdinalIgnoreCase);

              _logger.LogInformation(
                   "Dictionary cache loaded from disk: {Definitions} definitions, {Merged} merged, {Include} included, {Exclude} excluded.",
                   _definitions.Count, _merged.Count, _include.Count, _exclude.Count);
          }
        catch (Exception ex)
          {
              _logger.LogError(ex, "Failed to load dictionary files from disk at startup");
            throw;
          }
       }
    }