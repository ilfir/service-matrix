using Microsoft.AspNetCore.Mvc;

namespace service_matrix.Controllers;

/// <summary>
/// Controller for retrieving version information about the running application.
/// </summary>
[ApiController]
[Route("version")]
public class VersionController : ControllerBase
{
    private readonly ILogger<VersionController> _logger;

      /// <summary>
      /// Initializes a new instance of the <see cref="VersionController"/> class.
      /// </summary>
      /// <param name="logger">The logger.</param>
     public VersionController(ILogger<VersionController> logger)
      {
          _logger = logger;
      }

      /// <summary>
      /// Returns comprehensive version information about the application.
      /// </summary>
      /// <returns>A JSON object containing SHA, .NET framework version, and environment name.</returns>
      /// <response value="Ok">Returns when the operation completes successfully.</response>
     [HttpGet]
    public IActionResult Get()
       {
          _logger.LogInformation("Retrieving version information.");
         var result = new 
             { 
              Sha = ConfigurationService.GitSha,
              FrameworkDescription = Environment.Version.ToString(),
              EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"
             };
          _logger.LogInformation("Version information retrieved successfully. SHA={Sha}, Environment={Environment}", result.Sha, result.EnvironmentName);
         return Ok(new { success = true, data = result });
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