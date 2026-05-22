using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.Features.Admin.Commands.CreateRole;
using ProjectManagement.Application.Wrappers;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjectManagement.API.Controllers.v1;

/// <summary>
/// Manages global system roles. Access is restricted to users with the Admin role.
/// </summary>
[ApiVersion("1.0")]
[Authorize(Roles = "Admin")]
public sealed class RolesController(ISender mediator) : ApiControllerBase
{
    /// <summary>
    /// Creates a new global security role.
    /// </summary>
    /// <param name="role">The name of the role to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A confirmation of role creation.</returns>
    /// <response code="200">Role created successfully.</response>
    /// <response code="400">Validation failed or role already exists.</response>
    /// <response code="403">Forbidden. Only administrators can perform this action.</response>
    [HttpPost("{role}")]
    [SwaggerOperation(Summary = "Create Role", Description = "Creates a new global security role.")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        string role,
        CancellationToken cancellationToken)
    {
        var command = new CreateRoleCommand(role);
        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
