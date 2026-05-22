using MediatR;
using ProjectManagement.Application.DTOs.Tasks;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Tasks.Queries.GetTasksByProject;

/// <summary>
/// Query to retrieve all tasks for a specific project with pagination.
/// </summary>
public sealed record GetTasksByProjectQuery(
    Guid ProjectId,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<PaginatedResponse<TaskDto>>;
