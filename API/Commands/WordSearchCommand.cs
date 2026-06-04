/// <summary>
/// Command for performing a word search on a letter matrix.
/// </summary>
/// <param name="MaxLength">Maximum length of words to search for (1-100).</param>
/// <param name="MinLength">Minimum length of words to search for (1-100).</param>
/// <param name="MaxWords">Maximum number of words to return in the result.</param>
/// <param name="LettersMatrix">The letter matrix to search within.</param>
public record WordSearchCommand(int MaxLength, int MinLength, int MaxWords, List<List<string>> LettersMatrix);
