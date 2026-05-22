using MediatR;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Tasks.Commands.DeleteTask;

/// <summary>
/// Command to delete a task by Id.
/// </summary>
public sealed record DeleteTaskCommand(Guid Id) : IRequest<Result<bool>>;
