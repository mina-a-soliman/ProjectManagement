using ProjectManagement.API.Middleware;

namespace ProjectManagement.API.Extensions;

/// <summary>
/// Extension methods for configuring the middleware pipeline.
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// Adds custom middleware to the application pipeline.
    /// </summary>
    public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<RequestResponseLoggingMiddleware>();

        return app;
    }
}
