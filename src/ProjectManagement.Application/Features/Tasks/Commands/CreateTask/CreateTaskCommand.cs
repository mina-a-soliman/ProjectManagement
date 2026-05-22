using MediatR;
using ProjectManagement.Application.DTOs.Tasks;
using ProjectManagement.Application.Wrappers;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Features.Tasks.Commands.CreateTask;

/// <summary>
/// Command to create a new task within a project.
/// </summary>
public sealed record CreateTaskCommand(
    string Title,
    string? Description,
    ProjectTaskStatus Status,
    DateTime? DueDate,
    TaskPriority Priority,
    Guid ProjectId,
    Guid? AssignedUserId = null) : IRequest<Result<TaskDto>>;
