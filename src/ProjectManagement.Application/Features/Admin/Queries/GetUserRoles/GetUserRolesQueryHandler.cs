using MediatR;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Admin.Queries.GetUserRoles;

public sealed class GetUserRolesQueryHandler(
    IIdentityService identityService,
    ICurrentUserService currentUserService) : IRequestHandler<GetUserRolesQuery, Result<List<string>>>
{
    public async Task<Result<List<string>>> Handle(
        GetUserRolesQuery request,
        CancellationToken cancellationToken)
    {
        if (!currentUserService.IsInRole("Admin"))
            throw new UnauthorizedAccessException("Only administrators can view user roles.");

        return await identityService.GetUserRolesAsync(request.UserId, cancellationToken);
    }
}
