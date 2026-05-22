using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.Features.Admin.Commands.AssignUserToProject;
using ProjectManagement.Application.Features.Admin.Commands.RemoveUserFromProject;
using ProjectManagement.Application.Features.Admin.Commands.AssignTaskToUser;
using ProjectManagement.Application.Features.Admin.Commands.CreateUser;
using ProjectManagement.Application.Features.Admin.Commands.CreateRole;
using ProjectManagement.Application.Features.Admin.Commands.AddUserRole;
using ProjectManagement.Application.Features.Admin.Commands.RemoveUserRole;
using ProjectManagement.Application.Features.Admin.Queries.GetUserRoles;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.API.Controllers.v1;

/// <summary>
/// Manages specialized administrative operations. Endpoints require Admin role.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = "Admin")]
[Produces("application/json")]
public sealed class AdminController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// Assigns a user to a project.
    /// </summary>
    [HttpPost("projects/{projectId:guid}/users/{userId:guid}")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AssignUserToProject(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new AssignUserToProjectCommand(projectId, userId);
        var result = await mediator.Send(command, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Removes a user from a project.
    /// </summary>
    [HttpDelete("projects/{projectId:guid}/users/{userId:guid}")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveUserFromProject(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveUserFromProjectCommand(projectId, userId);
        var result = await mediator.Send(command, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Assigns a task to a user.
    /// </summary>
    [HttpPost("tasks/{taskId:guid}/assign/{userId:guid}")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AssignTaskToUser(
        Guid taskId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new AssignTaskToUserCommand(taskId, userId);
        var result = await mediator.Send(command, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Assigns a role to a user.
    /// </summary>
    [HttpPost("users/{userId:guid}/roles/{role}")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddUserRole(
        Guid userId,
        string role,
        CancellationToken cancellationToken)
    {
        var command = new AddUserRoleCommand(userId, role);
        var result = await mediator.Send(command, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Removes a role from a user.
    /// </summary>
    [HttpDelete("users/{userId:guid}/roles/{role}")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveUserRole(
        Guid userId,
        string role,
        CancellationToken cancellationToken)
    {
        var command = new RemoveUserRoleCommand(userId, role);
        var result = await mediator.Send(command, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Views user roles.
    /// </summary>
    [HttpGet("users/{userId:guid}/roles")]
    [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<List<string>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserRoles(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var query = new GetUserRolesQuery(userId);
        var result = await mediator.Send(query, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Creates a new role.
    /// </summary>
    [HttpPost("roles/{role}")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRole(
        string role,
        CancellationToken cancellationToken)
    {
        var command = new CreateRoleCommand(role);
        var result = await mediator.Send(command, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    [HttpPost("users")]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.Success ? CreatedAtAction(nameof(GetUserRoles), new { userId = result.Data }, result) : BadRequest(result);
    }
}
