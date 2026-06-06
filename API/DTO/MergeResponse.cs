namespace service_matrix.DTO;

/// <summary>
/// Response model for merge operations, containing counts of added and removed words.
/// </summary>
/// <param name="AddedCount">The number of words added during the merge operation.</param>
/// <param name="RemovedCount">The number of words removed during the merge operation.</param>
public record MergeResponse(int AddedCount, int RemovedCount);