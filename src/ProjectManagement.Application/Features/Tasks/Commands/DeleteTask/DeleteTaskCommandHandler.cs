using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Tasks.Commands.DeleteTask;

/// <summary>
/// Handles deleting a task. Verifies ownership through the parent project.
/// </summary>
public sealed class DeleteTaskCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService) : IRequestHandler<DeleteTaskCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteTaskCommand request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        if (!currentUserService.IsInRole("Admin"))
            throw new UnauthorizedAccessException("Only administrators can delete tasks.");

        var task = await dbContext.ProjectTasks
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (task is null)
            return Result<bool>.FailureResponse("Task not found or access denied.");

        dbContext.ProjectTasks.Remove(task);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<bool>.SuccessResponse(true, "Task deleted successfully.");
    }
}
