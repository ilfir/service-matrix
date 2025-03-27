namespace service_matrix.Queries;

public class GetWordsQuery
{
    public bool Include { get; }

    public GetWordsQuery(bool include)
    {
        Include = include;
    }
}