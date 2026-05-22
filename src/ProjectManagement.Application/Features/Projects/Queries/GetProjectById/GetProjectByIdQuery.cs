using MediatR;
using ProjectManagement.Application.DTOs.Projects;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Projects.Queries.GetProjectById;

/// <summary>
/// Query to retrieve a specific project by Id.
/// </summary>
public sealed record GetProjectByIdQuery(Guid Id) : IRequest<Result<ProjectDto>>;
