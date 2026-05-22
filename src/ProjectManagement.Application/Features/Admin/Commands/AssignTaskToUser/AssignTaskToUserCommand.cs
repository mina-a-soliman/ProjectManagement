using MediatR;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Admin.Commands.AssignTaskToUser;

public sealed record AssignTaskToUserCommand(Guid TaskId, Guid? UserId) : IRequest<Result<bool>>;
