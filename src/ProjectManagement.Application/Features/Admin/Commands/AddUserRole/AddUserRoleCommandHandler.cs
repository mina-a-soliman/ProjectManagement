using MediatR;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Admin.Commands.AddUserRole;

public sealed class AddUserRoleCommandHandler(
    IIdentityService identityService,
    ICurrentUserService currentUserService) : IRequestHandler<AddUserRoleCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        AddUserRoleCommand request,
        CancellationToken cancellationToken)
    {
        if (!currentUserService.IsInRole("Admin"))
            throw new UnauthorizedAccessException("Only administrators can manage user roles.");

        return await identityService.AddUserToRoleAsync(request.UserId, request.Role, cancellationToken);
    }
}
