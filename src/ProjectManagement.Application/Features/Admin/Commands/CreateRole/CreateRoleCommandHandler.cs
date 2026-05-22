using MediatR;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Admin.Commands.CreateRole;

public sealed class CreateRoleCommandHandler(
    IIdentityService identityService,
    ICurrentUserService currentUserService) : IRequestHandler<CreateRoleCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        CreateRoleCommand request,
        CancellationToken cancellationToken)
    {
        if (!currentUserService.IsInRole("Admin"))
            throw new UnauthorizedAccessException("Only administrators can create roles.");

        return await identityService.CreateRoleAsync(request.RoleName, cancellationToken);
    }
}
