namespace service_matrix.Queries;

/// <summary>
/// Query for retrieving words from the include or exclude list.
/// </summary>
public class GetWordsQuery
{
     /// <summary>
     /// Whether to retrieve included or excluded words.
     /// </summary>
    public bool Include { get; }

     /// <summary>
     /// Initializes a new instance of the <see cref="GetWordsQuery"/> class.
     /// </summary>
     /// <param name="include">True to retrieve included words, false for excluded words.</param>
    public GetWordsQuery(bool include)
     {
        Include = include;
     }
}
