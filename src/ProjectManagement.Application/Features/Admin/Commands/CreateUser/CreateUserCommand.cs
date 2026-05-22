using MediatR;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Admin.Commands.CreateUser;

public sealed record CreateUserCommand(
    string Email,
    string Password,
    string FullName,
    List<string> Roles) : IRequest<Result<Guid>>;
