namespace service_matrix.Queries;

/// <summary>
/// 
/// </summary>
/// <param name="Word"></param>
/// <param name="ExactMatch"></param>
public record LookupWordQuery(string Word, bool ExactMatch);