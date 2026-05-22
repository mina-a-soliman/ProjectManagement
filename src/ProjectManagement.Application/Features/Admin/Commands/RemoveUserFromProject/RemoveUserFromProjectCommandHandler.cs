using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Admin.Commands.RemoveUserFromProject;

public sealed class RemoveUserFromProjectCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService) : IRequestHandler<RemoveUserFromProjectCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        RemoveUserFromProjectCommand request,
        CancellationToken cancellationToken)
    {
        if (!currentUserService.IsInRole("Admin"))
            throw new UnauthorizedAccessException("Only administrators can remove users from projects.");

        var projectUser = await dbContext.ProjectUsers
            .FirstOrDefaultAsync(pu => pu.ProjectId == request.ProjectId && pu.UserId == request.UserId, cancellationToken);

        if (projectUser is null)
            return Result<bool>.FailureResponse("User assignment not found on this project.");

        dbContext.ProjectUsers.Remove(projectUser);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<bool>.SuccessResponse(true, "User removed from project successfully.");
    }
}
