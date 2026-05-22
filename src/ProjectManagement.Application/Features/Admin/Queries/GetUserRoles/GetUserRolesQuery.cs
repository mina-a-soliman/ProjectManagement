using MediatR;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Features.Admin.Queries.GetUserRoles;

public sealed record GetUserRolesQuery(Guid UserId) : IRequest<Result<List<string>>>;
