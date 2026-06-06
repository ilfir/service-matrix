namespace service_matrix.Commands;

/// <summary>
/// Command for adding or removing words from the include/exclude lists.
/// </summary>
/// <param name="Words">The list of words to include or exclude.</param>
/// <param name="Include">Whether to include (true) or exclude (false) the words.</param>
public record UpdateWordsCommand(List<string> Words, bool Include);
