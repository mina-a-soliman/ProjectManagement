using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Admin.Commands.AssignTaskToUser;

public sealed class AssignTaskToUserCommandHandler(
    IApplicationDbContext dbContext,
    IIdentityService identityService,
    ICurrentUserService currentUserService) : IRequestHandler<AssignTaskToUserCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        AssignTaskToUserCommand request,
        CancellationToken cancellationToken)
    {
        var adminId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        if (!currentUserService.IsInRole("Admin"))
            throw new UnauthorizedAccessException("Only administrators can assign tasks to users.");

        var task = await dbContext.ProjectTasks
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

        if (task is null)
            return Result<bool>.FailureResponse("Task not found.");

        if (request.UserId.HasValue)
        {
            var userExists = await identityService.UserExistsAsync(request.UserId.Value, cancellationToken);
            if (!userExists)
                return Result<bool>.FailureResponse("User not found.");
        }

        task.AssignUser(request.UserId, adminId);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<bool>.SuccessResponse(true, request.UserId.HasValue ? "Task assigned successfully." : "Task unassigned successfully.");
    }
}
