namespace service_matrix.Options;

/// <summary>
/// Represents the Paths configuration section from appsettings.json.
/// </summary>
public class PathsOptions
{
    /// <summary>
    /// Gets or sets the path to the merged file.
    /// </summary>
    public string MergedFilePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path to the merged cleaned file.
    /// </summary>
    public string MergedCleanedFilePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path to the definitions file.
    /// </summary>
    public string DefinitionsFilePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path to the definitions backup file.
    /// </summary>
    public string DefinitionsBackupFilePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path to the include file.
    /// </summary>
    public string IncludeFilePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path to the exclude file.
    /// </summary>
    public string ExcludeFilePath { get; set; } = string.Empty;
}
