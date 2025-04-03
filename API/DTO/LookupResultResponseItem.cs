namespace service_matrix.DTO;

/// <summary>
/// 
/// </summary>
/// <param name="Word"></param>
/// <param name="Location"></param>
public record LookupResultResponseItem(string Word, string Location);

/// <summary>
/// 
/// </summary>
public enum WordLocation
{
    /// <summary>
    /// 
    /// </summary>
    Dictionary,
    /// <summary>
    /// 
    /// </summary>
    Merged,
    /// <summary>
    /// 
    /// </summary>
    Included,
    /// <summary>
    /// 
    /// </summary>
    Excluded,
    /// <summary>
    /// 
    /// </summary>
    Error
}