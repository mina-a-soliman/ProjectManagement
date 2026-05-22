using MediatR;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Admin.Commands.AddUserRole;

public sealed record AddUserRoleCommand(Guid UserId, string Role) : IRequest<Result<bool>>;
