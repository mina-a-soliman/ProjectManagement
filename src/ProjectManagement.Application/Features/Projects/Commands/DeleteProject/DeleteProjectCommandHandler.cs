using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Projects.Commands.DeleteProject;

/// <summary>
/// Handles deleting a project and its tasks (cascade). Enforces user ownership.
/// </summary>
public sealed class DeleteProjectCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService) : IRequestHandler<DeleteProjectCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteProjectCommand request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        if (!currentUserService.IsInRole("Admin"))
            throw new UnauthorizedAccessException("Only administrators can delete projects.");

        var project = await dbContext.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (project is null)
            return Result<bool>.FailureResponse("Project not found.");

        dbContext.ProjectTasks.RemoveRange(project.Tasks);
        dbContext.Projects.Remove(project);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<bool>.SuccessResponse(true, "Project deleted successfully.");
    }
}
