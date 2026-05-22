using MediatR;
using ProjectManagement.Application.DTOs.Projects;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Projects.Commands.UpdateProject;

/// <summary>
/// Command to update an existing project.
/// </summary>
public sealed record UpdateProjectCommand(
    Guid Id,
    string Name,
    string? Description) : IRequest<Result<ProjectDto>>;
