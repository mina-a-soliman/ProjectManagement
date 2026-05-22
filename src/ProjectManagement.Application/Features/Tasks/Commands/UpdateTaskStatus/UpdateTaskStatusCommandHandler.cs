using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.DTOs.Tasks;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;

/// <summary>
/// Handles updating a task's status. Ensures the task belongs to a project owned by the user.
/// </summary>
public sealed class UpdateTaskStatusCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<UpdateTaskStatusCommand, Result<TaskDto>>
{
    public async Task<Result<TaskDto>> Handle(
        UpdateTaskStatusCommand request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var task = await dbContext.ProjectTasks
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (task is null)
            return Result<TaskDto>.FailureResponse("Task not found.");

        var isAdmin = currentUserService.IsInRole("Admin");
        if (!isAdmin && task.AssignedUserId != userId)
            throw new UnauthorizedAccessException("Only the assigned user or an administrator can update the task status.");

        task.UpdateStatus(request.Status, userId);
        await dbContext.SaveChangesAsync(cancellationToken);

        var dto = mapper.Map<TaskDto>(task);

        return Result<TaskDto>.SuccessResponse(dto, "Task status updated successfully.");
    }
}
