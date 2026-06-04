namespace service_matrix.DTO;

/// <summary>
/// Response model for word lookup results, containing the matched word and its location.
/// </summary>
/// <param name="Word">The matched word found in a dictionary source.</param>
/// <param name="Location">The source where the word was found (e.g., Dictionary, Merged, Included, Excluded).</param>
public record LookupResultResponseItem(string Word, string Location);

/// <summary>
/// Enumerates the possible sources where a word can be located.
/// </summary>
public enum WordLocation
{
    /// <summary>
    /// The word was found in the main definitions dictionary.
    /// </summary>
    Dictionary,
    /// <summary>
    /// The word was found in the merged dictionary file.
    /// </summary>
    Merged,
    /// <summary>
    /// The word was found in the include list.
    /// </summary>
    Included,
    /// <summary>
    /// The word was found in the exclude list.
    /// </summary>
    Excluded,
    /// <summary>
    /// An error occurred during lookup.
    /// </summary>
    Error
}
