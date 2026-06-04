using System.ComponentModel.DataAnnotations;

namespace service_matrix.DTO;

/// <summary>
/// Request model for the update words endpoint.
/// </summary>
public class UpdateWordsRequest
{
     /// <summary>
     /// List of words to include or exclude. Must contain at least one word.
     /// </summary>
     [Required(ErrorMessage = "Words list is required.")]
     [MinLength(1, ErrorMessage = "Words list must contain at least one word.")]
    public List<string> Words { get; set; } = new();

     /// <summary>
     /// Whether to include (true) or exclude (false) the words.
     /// </summary>
    public bool Include { get; set; }
}
