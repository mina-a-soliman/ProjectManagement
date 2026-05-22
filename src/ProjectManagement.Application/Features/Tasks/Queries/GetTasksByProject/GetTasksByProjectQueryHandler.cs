using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.DTOs.Tasks;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Tasks.Queries.GetTasksByProject;

/// <summary>
/// Handles retrieving tasks for a specific project owned by the current user.
/// </summary>
public sealed class GetTasksByProjectQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<GetTasksByProjectQuery, PaginatedResponse<TaskDto>>
{
    public async Task<PaginatedResponse<TaskDto>> Handle(
        GetTasksByProjectQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var isAdmin = currentUserService.IsInRole("Admin");
        var hasAccess = isAdmin || await dbContext.ProjectUsers
            .AnyAsync(pu => pu.ProjectId == request.ProjectId && pu.UserId == userId, cancellationToken);

        if (!hasAccess)
            return PaginatedResponse<TaskDto>.Create([], 0, request.PageNumber, request.PageSize,
                "Project not found or access denied.");

        var query = dbContext.ProjectTasks
            .Where(t => t.ProjectId == request.ProjectId && (isAdmin || t.AssignedUserId == userId))
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
            "Tasks retrieved successfully.");
    }
}
