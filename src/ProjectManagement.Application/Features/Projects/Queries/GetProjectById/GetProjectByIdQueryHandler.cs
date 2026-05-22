using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.DTOs.Projects;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Projects.Queries.GetProjectById;

/// <summary>
/// Handles retrieving a single project by Id, scoped to the authenticated user.
/// </summary>
public sealed class GetProjectByIdQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<GetProjectByIdQuery, Result<ProjectDto>>
{
    public async Task<Result<ProjectDto>> Handle(
        GetProjectByIdQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var project = await dbContext.Projects
            .Include(p => p.Tasks)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (project is null)
            return Result<ProjectDto>.FailureResponse("Project not found.");

        if (!currentUserService.IsInRole("Admin"))
        {
            var hasAccess = await dbContext.ProjectUsers
                .AnyAsync(pu => pu.ProjectId == request.Id && pu.UserId == userId, cancellationToken);
            if (!hasAccess)
                throw new UnauthorizedAccessException("Access to this project is denied.");
        }

        var dto = mapper.Map<ProjectDto>(project);

        return Result<ProjectDto>.SuccessResponse(dto, "Project retrieved successfully.");
    }
}
