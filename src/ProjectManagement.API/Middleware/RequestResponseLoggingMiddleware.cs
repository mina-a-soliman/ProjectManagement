using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.API.Middleware;

/// <summary>
/// Middleware that logs every API request and response to the ApiLogs database table.
/// Handles request body buffering, response stream capture, sensitive field masking,
/// and payload truncation.
/// </summary>
public sealed class RequestResponseLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestResponseLoggingMiddleware> logger)
{
    private const int MaxPayloadLength = 4096;

    private static readonly HashSet<string> SensitiveFields =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "password", "confirmPassword", "token", "accessToken",
            "refreshToken", "secretKey", "authorization"
        };

    public async Task InvokeAsync(HttpContext context, IApplicationDbContext dbContext)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid();
        var correlationId = requestId.ToString();
        context.Items["CorrelationId"] = correlationId;

        string? requestBody = null;
        string? responseBody = null;
        string? exception = null;
        var statusCode = 500;

        try
        {
            // Enable request body buffering for reading
            context.Request.EnableBuffering();

            // Read and mask request body
            requestBody = await ReadAndMaskRequestBody(context.Request);

            // Capture response body
            var originalBodyStream = context.Response.Body;

            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            await next(context);

            statusCode = context.Response.StatusCode;

            // Read response body
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
            responseBodyStream.Seek(0, SeekOrigin.Begin);

            // Copy response back to original stream
            await responseBodyStream.CopyToAsync(originalBodyStream);

            // Mask and truncate response
            responseBody = MaskSensitiveData(responseBody);
            responseBody = TruncatePayload(responseBody);
        }
        catch (Exception ex)
        {
            exception = $"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}";
            throw;
        }
        finally
        {
            stopwatch.Stop();

            try
            {
                var userId = GetUserId(context);

                var apiLog = new ApiLog
                {
                    Id = requestId,
                    Method = context.Request.Method,
                    Url = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}",
                    Request = requestBody,
                    Response = responseBody,
                    Exception = exception,
                    StatusCode = statusCode,
                    UserId = userId,
                    ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
                    CorrelationId = correlationId,
                    CreatedAt = DateTime.UtcNow
                };

                dbContext.ApiLogs.Add(apiLog);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception logEx)
            {
                // Never break the request pipeline due to logging failures
                logger.LogError(logEx, "Failed to persist API log for {Method} {Url}",
                    context.Request.Method, context.Request.Path);
            }
        }
    }

    private static async Task<string?> ReadAndMaskRequestBody(HttpRequest request)
    {
        // Skip binary/file uploads
        if (request.ContentType is not null &&
            (request.ContentType.Contains("multipart") ||
             request.ContentType.Contains("octet-stream")))
        {
            return "[Binary content omitted]";
        }

        request.Body.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();

        request.Body.Seek(0, SeekOrigin.Begin);

        if (string.IsNullOrWhiteSpace(body))
            return null;

        body = MaskSensitiveData(body);
        return TruncatePayload(body);
    }

    private static string MaskSensitiveData(string? data)
    {
        if (string.IsNullOrWhiteSpace(data))
            return string.Empty;

        try
        {
            using var document = JsonDocument.Parse(data);
            var root = document.RootElement;

            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = false });

            MaskJsonElement(writer, root);

            writer.Flush();
            return Encoding.UTF8.GetString(stream.ToArray());
        }
        catch (JsonException)
        {
            // Not JSON — use regex fallback
            foreach (var field in SensitiveFields)
            {
                data = SensitiveFieldRegex(field)
                    .Replace(data, $"\"{field}\":\"***REDACTED***\"");
            }

            return data;
        }
    }

    private static void MaskJsonElement(Utf8JsonWriter writer, JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                writer.WriteStartObject();
                foreach (var property in element.EnumerateObject())
                {
                    writer.WritePropertyName(property.Name);
                    if (SensitiveFields.Contains(property.Name))
                        writer.WriteStringValue("***REDACTED***");
                    else
                        MaskJsonElement(writer, property.Value);
                }
                writer.WriteEndObject();
                break;

            case JsonValueKind.Array:
                writer.WriteStartArray();
                foreach (var item in element.EnumerateArray())
                    MaskJsonElement(writer, item);
                writer.WriteEndArray();
                break;

            default:
                element.WriteTo(writer);
                break;
        }
    }

    private static string TruncatePayload(string? payload)
    {
        if (string.IsNullOrEmpty(payload) || payload.Length <= MaxPayloadLength)
            return payload ?? string.Empty;

        return string.Concat(payload.AsSpan(0, MaxPayloadLength), "...[TRUNCATED]");
    }

    private static Guid? GetUserId(HttpContext context)
    {
        var claim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out var userId) ? userId : null;
    }

    private static Regex SensitiveFieldRegex(string field) =>
        new($"\"{field}\"\\s*:\\s*\"[^\"]*\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);
}
