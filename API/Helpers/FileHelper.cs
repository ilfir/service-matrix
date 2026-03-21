using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using service_matrix.Options;

namespace service_matrix.Helpers;

/// <summary>
/// File Helper
/// </summary>
public class FileHelper
{
    private readonly PathsOptions _pathsOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileHelper"/> class.
    /// </summary>
    /// <param name="pathsOptions">The paths options.</param>
    public FileHelper(IOptions<PathsOptions> pathsOptions)
    {
        _pathsOptions = pathsOptions.Value;
    }

    /// <summary>
    /// Reads a file asynchronously.
    /// </summary>
    /// <param name="pathKeyOrPath">The path key or full path.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task<IEnumerable<string>> ReadFileAsync(string pathKeyOrPath)
    {
        string path;
        if (pathKeyOrPath.Contains('/'))
        {
            path = pathKeyOrPath;
        }
        else
        {
            path = _pathsOptions.GetType().GetProperty(pathKeyOrPath)?.GetValue(_pathsOptions, null) as string ?? throw new InvalidOperationException("Path configuration not found");
        }
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"File not found at path: {path}");
        }
        return await File.ReadAllLinesAsync(path);
    }
    
    /// <summary>
    /// Writes new contents to a file asynchronously.
    /// </summary>
    /// <param name="contents">The contents.</param>
    /// <param name="pathKeyOrPath">The path key or full path.</param>
    public async Task WriteFileNewContents(IEnumerable<string> contents, string pathKeyOrPath)
    {
        string path;
        if (pathKeyOrPath.Contains('/'))
        {
            path = pathKeyOrPath;
        }
        else
        {
            path = _pathsOptions.GetType().GetProperty(pathKeyOrPath)?.GetValue(_pathsOptions, null) as string ?? throw new InvalidOperationException("Path configuration not found");
        }
        await File.WriteAllLinesAsync(path, contents);
    }

    /// <summary>
    /// Writes contents to a file asynchronously, appending to the existing contents if the file already exists.
    /// </summary>
    /// <param name="contents">The contents.</param>
    /// <param name="pathKeyOrPath">The path key or full path.</param>
    public async Task WriteFileAppend(IEnumerable<string> contents, string pathKeyOrPath)
    {
        string path;
        if (pathKeyOrPath.Contains('/'))
        {
            path = pathKeyOrPath;
        }
        else
        {
            path = _pathsOptions.GetType().GetProperty(pathKeyOrPath)?.GetValue(_pathsOptions, null) as string ?? throw new InvalidOperationException("Path configuration not found");
        }
        await File.AppendAllLinesAsync(path, contents);
    }
}