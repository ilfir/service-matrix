using Microsoft.AspNetCore.Mvc;

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
/// The GitSha is injected at build/deploy time via Dockerfile ARG GIT_SHA.
/// </summary>
public static class ConfigurationService
{
    public static readonly string GitSha = ReadGitSha();

    private static string ReadGitSha()
     {
        var shaFilePath = Path.Combine(AppContext.BaseDirectory, ".git-sha");
        if (File.Exists(shaFilePath))
         {
            var sha = File.ReadAllText(shaFilePath).Trim();
            if (!string.IsNullOrEmpty(sha) && sha.Length == 40)
             {
                return sha;
             }
         }

        return "unknown";
      }
}