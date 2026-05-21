namespace ProjectManagement.Domain.Entities;

/// <summary>
/// Represents an API request/response log entry for auditing and diagnostics.
/// </summary>
public class ApiLog
{
    public Guid Id { get; set; }

    public string Method { get; set; } = default!;

    public string Url { get; set; } = default!;

    public string? Request { get; set; }

    public string? Response { get; set; }

    public string? Exception { get; set; }

    public int StatusCode { get; set; }

    public Guid? UserId { get; set; }

    public long ExecutionTimeMs { get; set; }

    public string? CorrelationId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
