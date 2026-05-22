using MediatR;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Admin.Commands.AssignUserToProject;

public sealed record AssignUserToProjectCommand(Guid ProjectId, Guid UserId) : IRequest<Result<bool>>;
