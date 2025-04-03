namespace service_matrix.Commands;

/// <summary>
/// 
/// </summary>
/// <param name="Words"></param>
/// <param name="Include"></param>
public record UpdateWordsCommand(List<string> Words, bool Include);
