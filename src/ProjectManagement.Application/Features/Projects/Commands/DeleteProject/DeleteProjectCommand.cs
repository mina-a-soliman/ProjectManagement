using MediatR;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Projects.Commands.DeleteProject;

/// <summary>
/// Command to delete a project by Id.
/// </summary>
public sealed record DeleteProjectCommand(Guid Id) : IRequest<Result<bool>>;
