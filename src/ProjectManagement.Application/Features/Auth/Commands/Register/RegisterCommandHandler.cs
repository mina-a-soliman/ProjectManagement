using MediatR;
using ProjectManagement.Application.DTOs.Auth;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    IIdentityService identityService) : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var registerRequest = new RegisterRequest(
            request.FullName,
            request.Email,
            request.Password,
            request.ConfirmPassword);

        return await identityService.RegisterAsync(registerRequest, cancellationToken);
    }
}
