using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.DTOs.Tasks;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Application.Wrappers;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Features.Tasks.Commands.CreateTask;

/// <summary>
/// Handles creating a task within a project owned by the authenticated user.
/// </summary>
public sealed class CreateTaskCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<CreateTaskCommand, Result<TaskDto>>
{
    public async Task<Result<TaskDto>> Handle(
        CreateTaskCommand request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        if (!currentUserService.IsInRole("Admin"))
            throw new UnauthorizedAccessException("Only administrators can create tasks.");

        // Verify the project exists
        var projectExists = await dbContext.Projects
            .AnyAsync(p => p.Id == request.ProjectId, cancellationToken);

        if (!projectExists)
            return Result<TaskDto>.FailureResponse("Project not found.");

        var task = ProjectTask.Create(
            request.Title,
            request.Description,
            request.Status,
            request.DueDate,
            request.Priority,
            request.ProjectId,
            userId,
            request.AssignedUserId);

        dbContext.ProjectTasks.Add(task);
        await dbContext.SaveChangesAsync(cancellationToken);

        var dto = mapper.Map<TaskDto>(task);

        return Result<TaskDto>.SuccessResponse(dto, "Task created successfully.");
    }
}
