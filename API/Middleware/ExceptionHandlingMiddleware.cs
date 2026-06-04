using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace service_matrix.Middleware;

/// <summary>
/// Middleware for handling exceptions that occur during request processing.
/// Catches unhandled exceptions and returns a consistent error response format.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger.</param>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An unexpected error occurred");
            await HandleExceptionAsync(context, exception);
        }
    }

    /// <summary>
    /// Handles the exception by writing a consistent error response.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, detail) = GetProblemDetailsForException(exception);

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var result = System.Text.Json.JsonSerializer.Serialize(new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
        });

        return context.Response.WriteAsync(result);
    }

    /// <summary>
    /// Gets the appropriate problem details based on the exception type.
    /// </summary>
    /// <param name="exception">The exception that occurred.</param>
    /// <returns>A tuple of status code, title, and detail.</returns>
    private static (HttpStatusCode, string, string) GetProblemDetailsForException(Exception exception)
    {
        return exception switch
        {
            OperationCanceledException => (HttpStatusCode.NotFound, "Cancelled", "The request was cancelled."),
            TimeoutException => (HttpStatusCode.RequestTimeout, "Timeout", "The request timed out."),
            ArgumentException => (HttpStatusCode.BadRequest, "Bad Request", "Invalid argument provided."),
            _ => (HttpStatusCode.InternalServerError, "Server Error", "An unexpected error occurred while processing the request.")
        };
    }
}