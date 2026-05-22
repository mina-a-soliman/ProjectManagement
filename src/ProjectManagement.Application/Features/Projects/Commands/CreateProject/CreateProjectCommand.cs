using MediatR;
using ProjectManagement.Application.DTOs.Projects;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Projects.Commands.CreateProject;

/// <summary>
/// Command to create a new project for the authenticated user.
/// </summary>
public sealed record CreateProjectCommand(
    string Name,
    string? Description) : IRequest<Result<ProjectDto>>;
