using MediatR;
using ProjectManagement.Application.DTOs.Tasks;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Tasks.Queries.GetMyTasks;

public sealed record GetMyTasksQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedResponse<TaskDto>>;
