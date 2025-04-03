namespace service_matrix.DTO;

public record LookupResultResponseItem(string Word, WordLocation Location);

public enum WordLocation
{
    Dictionary,
    Merged,
    Included,
    Excluded,
    Error
}