using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.DTOs.Projects;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Projects.Queries.GetMyProjects;

public sealed class GetMyProjectsQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<GetMyProjectsQuery, PaginatedResponse<ProjectDto>>
{
    public async Task<PaginatedResponse<ProjectDto>> Handle(
        GetMyProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var query = dbContext.Projects
            .Where(p => dbContext.ProjectUsers.Any(pu => pu.ProjectId == p.Id && pu.UserId == userId))
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
            "My projects retrieved successfully.");
    }
}
