using MediatR;
using ProjectManagement.Application.DTOs.Projects;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Projects.Queries.GetMyProjects;

public sealed record GetMyProjectsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedResponse<ProjectDto>>;
