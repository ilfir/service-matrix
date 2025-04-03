namespace service_matrix.DTO;

/// <summary>
/// 
/// </summary>
/// <param name="AddedCount"></param>
/// <param name="RemovedCount"></param>
public record MergeResponse(int AddedCount, int RemovedCount);