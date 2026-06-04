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
    /// Returns the git SHA of the currently running build.
    /// </summary>
    /// <returns>A JSON object containing the git SHA.</returns>
    [HttpGet("sha")]
    public IActionResult GetSha()
    {
        var sha = ConfigurationService.GitSha;
        return Ok(new { Sha = sha });
    }

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
    public const string GitSha = "7a4eada0f8a45b44fb4b493bc47c18b1d0dad201";
}