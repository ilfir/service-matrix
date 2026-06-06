using Microsoft.Extensions.Logging;
using service_matrix.DTO;

namespace service_matrix.Helpers;

/// <summary>
/// Provides programmatic validation for API requests. Implements <see cref="IRequestValidator"/> for dependency injection.
/// </summary>
public class RequestValidator : IRequestValidator
{
    private readonly ILogger<RequestValidator> _logger;

     /// <summary>
     /// Initializes a new instance of the <see cref="RequestValidator"/> class.
     /// </summary>
     /// <param name="logger">The logger.</param>
    public RequestValidator(ILogger<RequestValidator> logger)
    {
        _logger = logger;
    }

         /// <summary>
         /// Validates a search request's LettersMatrix.
         /// </summary>
         /// <param name="lettersMatrix">The letter matrix to validate.</param>
         /// <returns>A tuple of (isValid, errorMessage) indicating validation result.</returns>
    public (bool IsValid, string? ErrorMessage) ValidateSearchRequest(List<List<string>>? lettersMatrix)
    {
        if (lettersMatrix == null)
        {
            _logger.LogWarning("Search request validation failed: LettersMatrix is null.");
            return (false, "LettersMatrix must be provided and cannot be empty.");
        }

        if (lettersMatrix.Count == 0)
        {
            _logger.LogWarning("Search request validation failed: LettersMatrix is empty.");
            return (false, "LettersMatrix must be provided and cannot be empty.");
        }

        for (int i = 0; i < lettersMatrix.Count; i++)
        {
            if (lettersMatrix[i] == null || lettersMatrix[i].Count == 0)
            {
                _logger.LogWarning("Search request validation failed: Row {RowIndex} is null or empty.", i);
                return (false, "Each row in LettersMatrix must be provided and cannot be empty.");
            }
        }

        _logger.LogDebug("Search request validation passed for matrix with {RowCount} rows.", lettersMatrix.Count);
        return (true, null);
    }

         /// <summary>
         /// Validates an update words request.
         /// </summary>
         /// <param name="words">The words list to validate.</param>
         /// <returns>A tuple of (isValid, errorMessage) indicating validation result.</returns>
    public (bool IsValid, string? ErrorMessage) ValidateUpdateRequest(List<string>? words)
    {
        if (words == null)
        {
            _logger.LogWarning("Update request validation failed: Words list is null.");
            return (false, "Words list is required.");
        }

        if (words.Count < 1)
        {
            _logger.LogWarning("Update request validation failed: Words list must contain at least one word.");
            return (false, "Words list must contain at least one word.");
        }

        _logger.LogDebug("Update request validation passed for {WordCount} words.", words.Count);
        return (true, null);
    }
}