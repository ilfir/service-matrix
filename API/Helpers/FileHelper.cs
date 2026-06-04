using Microsoft.Extensions.Logging;

namespace service_matrix.Helpers;

/// <summary>
/// Provides file I/O utilities. Implements <see cref="IFileHelper"/> for dependency injection.
/// </summary>
public class FileHelper : IFileHelper
{
    private readonly ILogger<FileHelper> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileHelper"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public FileHelper(ILogger<FileHelper> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileHelper"/> class without logging (backward compatible).
    /// </summary>
    public FileHelper()
    {
        _logger = null;
    }

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
            _logger?.LogWarning("File not found: {FilePath}", Path.Combine(AppContext.BaseDirectory, directory, fileName));
            return Array.Empty<string>();
        }

        _logger?.LogDebug("Reading file: {FilePath}", filePath);
        var result = File.ReadLines(filePath).ToList();
        _logger?.LogDebug("Read {LineCount} lines from {FilePath}", result.Count, filePath);
        return result;
    }

    public async Task WriteFileNewContents(string directory, string fileName, IEnumerable<string> newContents)
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, directory, fileName);
        if (!File.Exists(filePath))
        {
            filePath = Path.Combine(directory, fileName);
        }

        _logger?.LogDebug("Writing file: {FilePath} with {Count} lines.", filePath, newContents.Count());
        await File.WriteAllLinesAsync(filePath, newContents);
        _logger?.LogDebug("Successfully wrote {LineCount} lines to {FilePath}", newContents.Count(), filePath);
    }

    public async Task WriteFileAppend(string directory, string fileName, IEnumerable<string> newContents)
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, directory, fileName);
        if (!File.Exists(filePath))
        {
            filePath = Path.Combine(directory, fileName);
        }
        _logger?.LogDebug("Appending to file: {FilePath}", filePath);
        await File.AppendAllLinesAsync(filePath, newContents);
        _logger?.LogDebug("Successfully appended lines to {FilePath}", filePath);
    }

    // ---- Static wrappers (backward-compatible) ----

    /// <summary>
    /// Reads all lines from a file using the default FileHelper instance.
    /// </summary>
    /// <param name="directory">The base directory relative to the app.</param>
    /// <param name="fileName">The file name.</param>
    /// <returns>The lines from the file, or an empty array if not found.</returns>
    public static IEnumerable<string> ReadFileAsync(string directory, string fileName)
    {
        var instance = new FileHelper();
        return instance.ReadFile(directory, fileName);
    }

    /// <summary>
    /// Writes the new contents to a file using the default FileHelper instance.
    /// </summary>
    /// <param name="newContents">The new contents to write.</param>
    /// <param name="directory">The base directory relative to the app.</param>
    /// <param name="fileName">The file name.</param>
    public static async Task WriteFileNewContents(IEnumerable<string> newContents, string directory, string fileName)
    {
        var instance = new FileHelper();
        await instance.WriteFileNewContents(directory, fileName, newContents);
    }

    /// <summary>
    /// Appends the contents to a file using the default FileHelper instance.
    /// </summary>
    /// <param name="newContents">The contents to append.</param>
    /// <param name="directory">The base directory relative to the app.</param>
    /// <param name="fileName">The file name.</param>
    public static async Task WriteFileAppend(IEnumerable<string> newContents, string directory, string fileName)
    {
        var instance = new FileHelper();
        await instance.WriteFileAppend(directory, fileName, newContents);
    }
}
