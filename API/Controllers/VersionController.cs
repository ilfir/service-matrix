using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace service_matrix.Controllers;

/// <summary>
/// Returns version information about the running application.
/// </summary>
[ApiController]
[Route("version")]
public class VersionController : ControllerBase
{
    /// <summary>
    /// Returns comprehensive version information.
    /// </summary>
    /// <returns>A JSON object with version details.</returns>
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new 
        { 
            Sha = ConfigurationService.GitSha,
            FrameworkDescription = Environment.Version.ToString(),
            EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"
        });
    }
}

/// <summary>
/// Provides access to build-time configuration values.
/// </summary>
public static class ConfigurationService
{
    public static readonly string GitSha = ReadGitSha();

    private static string ReadGitSha()
    {
        var shaFilePath = Path.Combine(AppContext.BaseDirectory, ".git-sha");
        if (File.Exists(shaFilePath))
        {
            return File.ReadAllText(shaFilePath).Trim();
        }
        
        // Fallback: try to get the current git SHA at runtime
        try
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = "rev-parse HEAD",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            using var process = Process.Start(processStartInfo);
            if (process != null)
             {
                process.WaitForExit(5000);
                if (!process.HasExited)
                 {
                    process.Kill();
                 }
                var sha = process.StandardOutput.ReadToEnd().Trim();
                if (!string.IsNullOrEmpty(sha))
                 return sha;
             }
          }
        catch
         {
             // Ignore exceptions in non-production environments
         }
        
        return "unknown";
    }
}