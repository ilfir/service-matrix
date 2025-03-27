namespace service_matrix.Commands;

public record UpdateWordsCommand(List<string> Words, bool Include);
