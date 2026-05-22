using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.DTOs.Tasks;

/// <summary>
/// Data transfer object for task data.
/// </summary>
public sealed record TaskDto(
    Guid Id,
    string Title,
    string? Description,
    ProjectTaskStatus Status,
    DateTime? DueDate,
    TaskPriority Priority,
    Guid ProjectId,
    Guid? AssignedUserId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
