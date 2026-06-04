namespace service_matrix.Helpers;

/// <summary>
/// Provides file I/O utilities. Implements <see cref="IFileHelper"/> for dependency injection.
/// </summary>
public class FileHelper : IFileHelper
{
         /// <summary>
          /// Reads all lines from a file.
          /// </summary>
          /// <param name="directory">The base directory relative to the app.</param>
          /// <param name="fileName">The file name.</param>
          /// <returns>The lines from the file, or an empty array if not found.</returns>
    public IEnumerable<string> ReadFile(string directory, string fileName)
         {
        string filePath = Path.Combine(AppContext.BaseDirectory, directory, fileName);
        if (!File.Exists(filePath))
             {
            filePath = Path.Combine(directory, fileName);
             }

        if (!File.Exists(filePath))
             {
            return Array.Empty<string>();
             }

        return File.ReadLines(filePath);
         }

    public async Task WriteFileNewContents(string directory, string fileName, IEnumerable<string> newContents)
         {
        string filePath = Path.Combine(AppContext.BaseDirectory, directory, fileName);
        if (!File.Exists(filePath))
             {
            filePath = Path.Combine(directory, fileName);
             }

        await File.WriteAllLinesAsync(filePath, newContents);
         }

    public async Task WriteFileAppend(string directory, string fileName, IEnumerable<string> newContents)
         {
        string filePath = Path.Combine(AppContext.BaseDirectory, directory, fileName);
        if (!File.Exists(filePath))
             {
            filePath = Path.Combine(directory, fileName);
             }
        await File.AppendAllLinesAsync(filePath, newContents);
         }

     // ---- Static wrappers (backward-compatible) ----

      /// <summary>
       /// 
       /// </summary>
       /// <param name="directory"></param>
       /// <param name="fileName"></param>
       /// <returns></returns>
    public static IEnumerable<string> ReadFileAsync(string directory, string fileName)
         {
        var instance = new FileHelper();
        return instance.ReadFile(directory, fileName);
         }

      /// <summary>
       /// 
       /// </summary>
       /// <param name="newContents"></param>
       /// <param name="directory"></param>
       /// <param name="fileName"></param>
    public static async Task WriteFileNewContents(IEnumerable<string> newContents, string directory, string fileName)
         {
        var instance = new FileHelper();
        await instance.WriteFileNewContents(directory, fileName, newContents);
         }

      /// <summary>
       /// 
       /// </summary>
       /// <param name="newContents"></param>
       /// <param name="directory"></param>
       /// <param name="fileName"></param>
    public static async Task WriteFileAppend(IEnumerable<string> newContents, string directory, string fileName)
         {
        var instance = new FileHelper();
        await instance.WriteFileAppend(directory, fileName, newContents);
         }
}