using MediatR;
using ProjectManagement.Application.DTOs.Auth;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(
    string FullName,
    string Email,
    string Password,
    string ConfirmPassword) : IRequest<Result<AuthResponse>>;
