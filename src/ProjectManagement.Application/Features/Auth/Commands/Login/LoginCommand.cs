using MediatR;
using ProjectManagement.Application.DTOs.Auth;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<Result<AuthResponse>>;


