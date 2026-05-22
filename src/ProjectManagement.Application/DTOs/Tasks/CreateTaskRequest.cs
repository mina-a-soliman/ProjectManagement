using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.DTOs.Tasks;

/// <summary>
/// Request model for creating a task.
/// </summary>
public sealed record CreateTaskRequest(
    string Title,
    string? Description,
    ProjectTaskStatus Status,
    DateTime? DueDate,
    TaskPriority Priority,
    Guid ProjectId);
