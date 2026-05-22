using MediatR;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Admin.Commands.CreateUser;

public sealed class CreateUserCommandHandler(
    IIdentityService identityService,
    ICurrentUserService currentUserService) : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        if (!currentUserService.IsInRole("Admin"))
            throw new UnauthorizedAccessException("Only administrators can create users.");

        return await identityService.CreateUserAsync(
            request.Email,
            request.Password,
            request.FullName,
            request.Roles,
            cancellationToken);
    }
}
