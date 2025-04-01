namespace service_matrix.Helpers;

public static class FileHelper
{
    public static IEnumerable<string> ReadFileAsync(string directory, string fileName )
    {
        string filePath = Path.Combine(AppContext.BaseDirectory,directory, fileName);
        if (!File.Exists(filePath))
        {
            filePath = Path.Combine(directory, fileName);
        }

        return File.ReadLines(filePath);
    }

    public static async Task WriteFileNewContents(IEnumerable<string> newContents, string directory, string fileName)
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, directory, fileName);
        if (!File.Exists(filePath))
        {
            filePath = Path.Combine(directory, fileName);
        }

        await File.WriteAllLinesAsync(filePath, newContents);
    }
}