    /// <summary>
    /// 
    /// </summary>
    /// <param name="MaxLength"></param>
    /// <param name="MinLength"></param>
    /// <param name="MaxWords"></param>
    /// <param name="LettersMatrix"></param>
    public record WordSearchCommand(int MaxLength, int MinLength, int MaxWords, List<List<string>> LettersMatrix);