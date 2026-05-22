using MediatR;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Admin.Commands.RemoveUserFromProject;

public sealed record RemoveUserFromProjectCommand(Guid ProjectId, Guid UserId) : IRequest<Result<bool>>;
