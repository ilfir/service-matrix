using System.ComponentModel.DataAnnotations;

namespace service_matrix.DTO;

/// <summary>
/// Request model for the word search endpoint.
/// </summary>
public class SearchRequest
{
    /// <summary>
    /// Maximum length of words to search for (1-100).
    /// </summary>
    [Range(1, 100, ErrorMessage = "MaxLength must be between 1 and 100.")]
    public int MaxLength { get; set; } = 5;

    /// <summary>
    /// Maximum number of words to return in the result (minimum 1).
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "MaxWords must be at least 1.")]
    public int MaxWords { get; set; } = 10;

    /// <summary>
    /// Minimum length of words to search for (1-100).
    /// </summary>
    [Range(1, 100, ErrorMessage = "MinLength must be between 1 and 100.")]
    public int MinLength { get; set; } = 1;

    /// <summary>
    /// The letter matrix to search within. Must be non-empty with all rows having equal length.
    /// </summary>
    [Required(ErrorMessage = "LettersMatrix is required.")]
    public List<List<string>>? LettersMatrix { get; set; }
}
