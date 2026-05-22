using MediatR;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Admin.Commands.CreateRole;

public sealed record CreateRoleCommand(string RoleName) : IRequest<Result<bool>>;
