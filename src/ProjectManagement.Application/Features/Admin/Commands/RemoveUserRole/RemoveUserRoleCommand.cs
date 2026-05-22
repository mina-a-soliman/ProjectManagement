using MediatR;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Admin.Commands.RemoveUserRole;

public sealed record RemoveUserRoleCommand(Guid UserId, string Role) : IRequest<Result<bool>>;
