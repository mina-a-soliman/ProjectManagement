using MediatR;
using ProjectManagement.Application.Auth.DTOs;
using ProjectManagement.Application.DTOs.Auth;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    IIdentityService identityService) : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var loginRequest = new LoginRequest(request.Email, request.Password);
        return await identityService.LoginAsync(loginRequest, cancellationToken);
    }
}
