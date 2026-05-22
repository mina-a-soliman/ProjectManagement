using MediatR;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Admin.Commands.RemoveUserRole;

public sealed class RemoveUserRoleCommandHandler(
    IIdentityService identityService,
    ICurrentUserService currentUserService) : IRequestHandler<RemoveUserRoleCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        RemoveUserRoleCommand request,
        CancellationToken cancellationToken)
    {
        if (!currentUserService.IsInRole("Admin"))
            throw new UnauthorizedAccessException("Only administrators can manage user roles.");

        return await identityService.RemoveUserFromRoleAsync(request.UserId, request.Role, cancellationToken);
    }
}
