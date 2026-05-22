using MediatR;
using ProjectManagement.Application.DTOs.Projects;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Projects.Queries.GetAllProjects;

/// <summary>
/// Query to retrieve all projects for the authenticated user with pagination.
/// </summary>
public sealed record GetAllProjectsQuery(
    int PageNumber = 1,
    int PageSize = 10) : IRequest<PaginatedResponse<ProjectDto>>;
