namespace service_matrix.Queries;

/// <summary>
/// Query for looking up a word or partial word across all dictionary sources.
/// </summary>
/// <param name="Word">The word or partial word to search for.</param>
/// <param name="ExactMatch">Whether to perform an exact match (true) or wildcard search (false).</param>
public record LookupWordQuery(string Word, bool ExactMatch);
