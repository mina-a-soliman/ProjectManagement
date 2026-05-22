using MediatR;
using ProjectManagement.Application.DTOs.Tasks;
using ProjectManagement.Application.Wrappers;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;

/// <summary>
/// Command to update a task's status.
/// </summary>
public sealed record UpdateTaskStatusCommand(
    Guid Id,
    ProjectTaskStatus Status) : IRequest<Result<TaskDto>>;
