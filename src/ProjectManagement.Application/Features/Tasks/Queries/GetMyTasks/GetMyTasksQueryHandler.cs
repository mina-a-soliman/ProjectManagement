using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.DTOs.Tasks;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Tasks.Queries.GetMyTasks;

public sealed class GetMyTasksQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<GetMyTasksQuery, PaginatedResponse<TaskDto>>
{
    public async Task<PaginatedResponse<TaskDto>> Handle(
        GetMyTasksQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var query = dbContext.ProjectTasks
            .Where(t => t.AssignedUserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var tasks = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<TaskDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return PaginatedResponse<TaskDto>.Create(
            tasks,
            totalCount,
            request.PageNumber,
            request.PageSize,
            "My tasks retrieved successfully.");
    }
}
