namespace service_matrix.Helpers;

/// <summary>
/// 
/// </summary>
public static class FileHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static IEnumerable<string> ReadFileAsync(string directory, string fileName )
    {
        string filePath = Path.Combine(AppContext.BaseDirectory,directory, fileName);
        if (!File.Exists(filePath))
        {
            filePath = Path.Combine(directory, fileName);
        }

        return File.ReadLines(filePath);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newContents"></param>
    /// <param name="directory"></param>
    /// <param name="fileName"></param>
    public static async Task WriteFileNewContents(IEnumerable<string> newContents, string directory, string fileName)
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, directory, fileName);
        if (!File.Exists(filePath))
        {
            filePath = Path.Combine(directory, fileName);
        }

        await File.WriteAllLinesAsync(filePath, newContents);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="newContents"></param>
    /// <param name="directory"></param>
    /// <param name="fileName"></param>
    public static async Task WriteFileAppend(IEnumerable<string> newContents, string directory, string fileName)
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, directory, fileName);
        if (!File.Exists(filePath))
        {
            filePath = Path.Combine(directory, fileName);
        }
        await File.AppendAllLinesAsync(filePath, newContents);
    }
}