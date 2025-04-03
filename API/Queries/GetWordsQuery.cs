namespace service_matrix.Queries;

/// <summary>
/// 
/// </summary>
public class GetWordsQuery
{
    /// <summary>
    /// 
    /// </summary>
    public bool Include { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="include"></param>
    public GetWordsQuery(bool include)
    {
        Include = include;
    }
}