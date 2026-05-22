using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.DTOs.Projects;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Projects.Queries.GetAllProjects;

/// <summary>
/// Handles retrieving all projects for the current user with pagination support.
/// </summary>
public sealed class GetAllProjectsQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<GetAllProjectsQuery, PaginatedResponse<ProjectDto>>
{
    public async Task<PaginatedResponse<ProjectDto>> Handle(
        GetAllProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var isAdmin = currentUserService.IsInRole("Admin");
        var query = dbContext.Projects
            .Where(p => isAdmin || dbContext.ProjectUsers.Any(pu => pu.ProjectId == p.Id && pu.UserId == userId))
            .OrderByDescending(p => p.CreatedAt)
            .AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var projects = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<ProjectDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return PaginatedResponse<ProjectDto>.Create(
            projects,
            totalCount,
            request.PageNumber,
            request.PageSize,
            "Projects retrieved successfully.");
    }
}
