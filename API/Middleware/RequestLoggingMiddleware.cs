using System.Diagnostics;
using System.Text.Json;

namespace service_matrix.Middleware;

/// <summary>
/// Middleware for logging incoming HTTP requests and their processing duration.
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestLoggingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger.</param>
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware logic.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            
            _logger.LogInformation(
                "HTTP {Method} {Path} completed in {ElapsedMs}ms with status code {StatusCode}",
                context.Request.Method,
                context.Request.Path,
                stopwatch.Elapsed.TotalMilliseconds,
                context.Response.StatusCode);
        }
    }
}

/// <summary>
/// Extension methods for registering RequestLoggingMiddleware.
/// </summary>
public static class RequestLoggingMiddlewareExtensions
{
    /// <summary>
    /// Adds the request logging middleware to the HTTP pipeline.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <returns>The IApplicationBuilder with the middleware added.</returns>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}