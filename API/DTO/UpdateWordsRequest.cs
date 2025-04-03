namespace service_matrix.DTO;

/// <summary>
/// 
/// </summary>
public class UpdateWordsRequest
{
    /// <summary>
    /// 
    /// </summary>
    public List<string> Words { get; set; } = new();
    /// <summary>
    /// 
    /// </summary>
    public bool Include { get; set; }
}