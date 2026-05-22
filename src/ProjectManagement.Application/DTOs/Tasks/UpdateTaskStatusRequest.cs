using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.DTOs.Tasks;

/// <summary>
/// Request model for updating a task's status.
/// </summary>
public sealed record UpdateTaskStatusRequest(
    ProjectTaskStatus Status);
