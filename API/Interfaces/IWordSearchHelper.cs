namespace service_matrix.Helpers;

/// <summary>
/// Interface for word search operations, enabling dependency injection and testability.
/// </summary>
public interface IWordSearchHelper
{
    /// <summary>
    /// Perform the optimized backtracking word search.
    /// </summary>
    /// <returns>True if the word is found in the matrix, false otherwise.</returns>
    bool Search();

    /// <summary>
    /// Get the word locations found during search.
    /// </summary>
    /// <returns>Dictionary mapping word indices to their matrix coordinates.</returns>
    Dictionary<int, Dictionary<string, string>> GetFoundWord();

    /// <summary>
    /// Get the string formed by the search path.
    /// </summary>
    /// <returns>The concatenated string from the search path.</returns>
    string GetFoundString();

    /// <summary>
    /// Find all locations of the first letter in the matrix.
    /// </summary>
    /// <returns>List of coordinates where the first letter appears.</returns>
    List<(int, int)> FindLetterLocations();

    /// <summary>
    /// Check if the next letter in the word is a neighbor of the current position.
    /// </summary>
    /// <param name="iCurrentX">Current row position.</param>
    /// <param name="iCurrentY">Current column position.</param>
    /// <param name="arWord2">The word being searched.</param>
    /// <param name="iWordIndex">Current index in the word.</param>
    /// <param name="arLettersLoc">The letter matrix.</param>
    /// <returns>True if the next letter is a neighbor.</returns>
    bool IsNeighborToNextLetter(int iCurrentX, int iCurrentY, string[] arWord2, int iWordIndex, string[,] arLettersLoc);

    /// <summary>
    /// Get the current search path.
    /// </summary>
    /// <returns>List of coordinates in the current search path.</returns>
    List<(int, int)> GetCurrentPath();
}