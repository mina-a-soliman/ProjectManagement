using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.DTOs.Projects;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Projects.Commands.UpdateProject;

/// <summary>
/// Handles updating a project owned by the authenticated user.
/// </summary>
public sealed class UpdateProjectCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<UpdateProjectCommand, Result<ProjectDto>>
{
    public async Task<Result<ProjectDto>> Handle(
        UpdateProjectCommand request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        if (!currentUserService.IsInRole("Admin"))
            throw new UnauthorizedAccessException("Only administrators can update projects.");

        var project = await dbContext.Projects
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (project is null)
            return Result<ProjectDto>.FailureResponse("Project not found.");

        project.Update(request.Name, request.Description, userId);
        await dbContext.SaveChangesAsync(cancellationToken);

        var dto = mapper.Map<ProjectDto>(project);

        return Result<ProjectDto>.SuccessResponse(dto, "Project updated successfully.");
    }
}
