namespace service_matrix.DTO;

public class UpdateWordsRequest
{
    public List<string> Words { get; set; } = new();
    public bool Include { get; set; }
}