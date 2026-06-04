using service_matrix.DTO;

namespace service_matrix.Helpers;

/// <summary>
/// Interface for validating API requests. Provides programmatic validation beyond DataAnnotations.
/// </summary>
public interface IRequestValidator
{
     /// <summary>
     /// Validates a search request's LettersMatrix.
     /// </summary>
     /// <param name="lettersMatrix">The letter matrix to validate.</param>
     /// <returns>A tuple of (isValid, errorMessage) indicating validation result.</returns>
    (bool IsValid, string? ErrorMessage) ValidateSearchRequest(List<List<string>>? lettersMatrix);

     /// <summary>
     /// Validates an update words request.
     /// </summary>
     /// <param name="words">The words list to validate.</param>
     /// <returns>A tuple of (isValid, errorMessage) indicating validation result.</returns>
    (bool IsValid, string? ErrorMessage) ValidateUpdateRequest(List<string>? words);
}