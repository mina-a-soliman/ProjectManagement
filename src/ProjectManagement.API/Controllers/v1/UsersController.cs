using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.Features.Admin.Commands.CreateUser;
using ProjectManagement.Application.Features.Admin.Commands.AddUserRole;
using ProjectManagement.Application.Features.Admin.Commands.RemoveUserRole;
using ProjectManagement.Application.Features.Admin.Queries.GetUserRoles;
using ProjectManagement.Application.Wrappers;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjectManagement.API.Controllers.v1;

/// <summary>
/// Manages user accounts and role assignments. Access is restricted to users with the Admin role.
/// </summary>
[ApiVersion("1.0")]
[Authorize(Roles = "Admin")]
public sealed class UsersController(ISender mediator) : ApiControllerBase
{
    /// <summary>
    /// Creates a new user account with initial role assignments.
    /// </summary>
    /// <param name="command">User registration details and role assignments.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The unique identifier of the newly created user.</returns>
    /// <response code="201">User created successfully.</response>
    /// <response code="400">Validation failed or user already exists.</response>
    /// <response code="403">Forbidden. Only administrators can perform this action.</response>
    [HttpPost]
    [SwaggerOperation(Summary = "Create User", Description = "Creates a new user account with initial role assignments.")]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return HandleCreatedResult(result, nameof(GetRoles), new { userId = result.Data });
    }

    /// <summary>
    /// Retrieves all roles assigned to a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of roles assigned to the user.</returns>
    /// <response code="200">Roles retrieved successfully.</response>
    /// <response code="404">User not found.</response>
    /// <response code="403">Forbidden. Only administrators can perform this action.</response>
    [HttpGet("{userId:guid}/roles")]
    [SwaggerOperation(Summary = "Get User Roles", Description = "Retrieves all roles assigned to a specific user.")]
    [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetRoles(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var query = new GetUserRolesQuery(userId);
        var result = await mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Assigns a role to a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="role">The name of the role to assign.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A confirmation of the assignment.</returns>
    /// <response code="200">Role assigned successfully.</response>
    /// <response code="400">Validation failed or role already assigned.</response>
    /// <response code="404">User or role not found.</response>
    /// <response code="403">Forbidden. Only administrators can perform this action.</response>
    [HttpPost("{userId:guid}/roles/{role}")]
    [SwaggerOperation(Summary = "Assign Role to User", Description = "Assigns a role to a specific user.")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddRole(
        Guid userId,
        string role,
        CancellationToken cancellationToken)
    {
        var command = new AddUserRoleCommand(userId, role);
        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Removes a role from a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="role">The name of the role to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A confirmation of the removal.</returns>
    /// <response code="200">Role removed successfully.</response>
    /// <response code="400">Validation failed or role not assigned.</response>
    /// <response code="404">User or role not found.</response>
    /// <response code="403">Forbidden. Only administrators can perform this action.</response>
    [HttpDelete("{userId:guid}/roles/{role}")]
    [SwaggerOperation(Summary = "Remove Role from User", Description = "Removes a role from a specific user.")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveRole(
        Guid userId,
        string role,
        CancellationToken cancellationToken)
    {
        var command = new RemoveUserRoleCommand(userId, role);
        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
