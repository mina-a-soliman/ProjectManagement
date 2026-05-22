namespace ProjectManagement.Application.DTOs.Projects;

/// <summary>
/// Data transfer object for project data.
/// </summary>
public sealed record ProjectDto(
    Guid Id,
    string Name,
    string? Description,
    Guid UserId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int TaskCount);
