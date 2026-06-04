using System.Collections;

namespace service_matrix.Helpers;

/// <summary>
/// Interface for file operations, enabling dependency injection.
/// </summary>
public interface IFileHelper
{
       /// <summary>
       /// Reads all lines from a file.
       /// </summary>
       /// <param name="directory">The base directory relative to the app.</param>
       /// <param name="fileName">The file name.</param>
       /// <returns>The lines from the file, or an empty array if not found.</returns>
    IEnumerable<string> ReadFile(string directory, string fileName);

       /// <summary>
       /// Writes the new contents to a file.
       /// </summary>
       /// <param name="newContents">The new contents to write.</param>
       /// <param name="directory">The base directory relative to the app.</param>
       /// <param name="fileName">The file name.</param>
    Task WriteFileNewContents(string directory, string fileName, IEnumerable<string> newContents);

       /// <summary>
       /// Appends the contents to a file.
       /// </summary>
       /// <param name="newContents">The contents to append.</param>
       /// <param name="directory">The base directory relative to the app.</param>
       /// <param name="fileName">The file name.</param>
    Task WriteFileAppend(string directory, string fileName, IEnumerable<string> newContents);
}