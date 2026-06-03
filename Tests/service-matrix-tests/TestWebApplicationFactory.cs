using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Diagnostics;
using service_matrix.Controllers;

namespace service_matrix_tests;

/// <summary>
/// Test web application factory for configuring the test server.
/// Uses the API project's WordSearchController to establish the host context.
/// Configures the content root to point to the API project directory.
/// </summary>
public class TestWebApplicationFactory : WebApplicationFactory<WordSearchController>
{
    private static readonly Lazy<string> _apiDirectory = new(GetApiProjectDirectory);

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseContentRoot(_apiDirectory.Value);
    }

    private static string GetApiProjectDirectory()
    {
        // Get the directory of the test assembly and navigate to API folder
        var assemblyLocation = typeof(WordSearchController).Assembly.Location;
        var dir = Path.GetDirectoryName(assemblyLocation)!;
        
        // Navigate from bin/Debug/net10.0/ to API/ (4 levels up from test output)
        while (dir != null && !Path.GetFileName(dir)!.Equals("API", StringComparison.OrdinalIgnoreCase))
        {
            dir = Path.GetDirectoryName(dir)!;
        }
        
        return dir ?? Directory.GetCurrentDirectory();
    }
}