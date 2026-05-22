using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Application.Wrappers;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Features.Admin.Commands.AssignUserToProject;

public sealed class AssignUserToProjectCommandHandler(
    IApplicationDbContext dbContext,
    IIdentityService identityService,
    ICurrentUserService currentUserService) : IRequestHandler<AssignUserToProjectCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        AssignUserToProjectCommand request,
        CancellationToken cancellationToken)
    {
        var adminId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        if (!currentUserService.IsInRole("Admin"))
            throw new UnauthorizedAccessException("Only administrators can assign users to projects.");

        var project = await dbContext.Projects
            .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);

        if (project is null)
            return Result<bool>.FailureResponse("Project not found.");

        var userExists = await identityService.UserExistsAsync(request.UserId, cancellationToken);
        if (!userExists)
            return Result<bool>.FailureResponse("User not found.");

        var alreadyAssigned = await dbContext.ProjectUsers
            .AnyAsync(pu => pu.ProjectId == request.ProjectId && pu.UserId == request.UserId, cancellationToken);

        if (alreadyAssigned)
            return Result<bool>.FailureResponse("User is already assigned to this project.");

        var projectUser = new ProjectUser
        {
            ProjectId = request.ProjectId,
            UserId = request.UserId,
            AssignedAt = DateTime.UtcNow,
            AssignedBy = adminId
        };

        dbContext.ProjectUsers.Add(projectUser);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<bool>.SuccessResponse(true, "User assigned to project successfully.");
    }
}
