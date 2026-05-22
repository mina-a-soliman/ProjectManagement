using System.Net;
using System.Text.Json;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.API.Middleware;

/// <summary>
/// Global exception handling middleware that catches unhandled exceptions
/// and returns standardized error responses.
/// </summary>
public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning(ex, "Unauthorized access attempt.");
            await WriteErrorResponse(context, HttpStatusCode.Unauthorized, "Unauthorized access.");
        }
        catch (FluentValidation.ValidationException ex)
        {
            logger.LogWarning(ex, "Validation exception occurred.");
            var errors = ex.Errors.Select(e => e.ErrorMessage).ToList();
            await WriteErrorResponse(context, HttpStatusCode.BadRequest, "Validation failed.", errors);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred.");
            await WriteErrorResponse(context, HttpStatusCode.InternalServerError,
                "An unexpected error occurred. Please try again later.");
        }
    }

    private static async Task WriteErrorResponse(
        HttpContext context,
        HttpStatusCode statusCode,
        string message,
        List<string>? errors = null)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = Result.FailureResponse(message, errors);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }
}
