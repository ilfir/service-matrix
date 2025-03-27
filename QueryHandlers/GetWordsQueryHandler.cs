using service_matrix.Queries;

namespace service_matrix.QueryHandlers;

public class GetWordsQueryHandler
{
    public async Task<List<string>> Handle(GetWordsQuery query, CancellationToken cancellationToken)
    {
        var fileName = query.Include ? "include.txt" : "exclude.txt";
        string filePath = Path.Combine(AppContext.BaseDirectory, "data", fileName);

        // Read the file lines asynchronously
        var words = await File.ReadAllLinesAsync(filePath, cancellationToken);

        // Return the file contents as a list of strings
        return new List<string>(words);
    }
}