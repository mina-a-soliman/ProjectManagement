using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.DTOs.Tasks;
using ProjectManagement.Application.Features.Tasks.Commands.CreateTask;
using ProjectManagement.Application.Features.Tasks.Commands.DeleteTask;
using ProjectManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;
using ProjectManagement.Application.Features.Tasks.Queries.GetTasksByProject;
using ProjectManagement.Application.Features.Tasks.Queries.GetMyTasks;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.API.Controllers.v1;

/// <summary>
/// Manages task operations within projects. All endpoints require authentication.
/// Users can only access tasks within their own projects.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
[Produces("application/json")]
public sealed class TasksController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// Creates a new task within a project.
    /// </summary>
    /// <param name="command">Task creation details including project Id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created task.</returns>
    /// <response code="201">Task created successfully.</response>
    /// <response code="400">Validation failed or project not found.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<TaskDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<TaskDto>), StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        [FromBody] CreateTaskCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        return result.Success
            ? StatusCode(StatusCodes.Status201Created, result)
            : BadRequest(result);
    }

    /// <summary>
    /// Retrieves all tasks for a specific project with pagination.
    /// </summary>
    /// <param name="projectId">Project Id.</param>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Page size (default: 10).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of tasks.</returns>
    /// <response code="200">Tasks retrieved successfully.</response>
    [HttpGet("by-project/{projectId:guid}")]
    [ProducesResponseType(typeof(PaginatedResponse<TaskDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByProject(
        Guid projectId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTasksByProjectQuery(projectId, pageNumber, pageSize);
        var result = await mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves all tasks assigned to the authenticated user with pagination.
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Page size (default: 10).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of assigned tasks.</returns>
    /// <response code="200">Tasks retrieved successfully.</response>
    [HttpGet("my")]
    [ProducesResponseType(typeof(PaginatedResponse<TaskDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMy(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMyTasksQuery(pageNumber, pageSize);
        var result = await mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Updates a task's status.
    /// </summary>
    /// <param name="id">Task Id.</param>
    /// <param name="request">New task status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated task.</returns>
    /// <response code="200">Task status updated successfully.</response>
    /// <response code="404">Task not found.</response>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(Result<TaskDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<TaskDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateTaskStatusRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTaskStatusCommand(id, request.Status);
        var result = await mediator.Send(command, cancellationToken);

        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Deletes a task.
    /// </summary>
    /// <param name="id">Task Id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Deletion confirmation.</returns>
    /// <response code="200">Task deleted successfully.</response>
    /// <response code="404">Task not found.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteTaskCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        return result.Success ? Ok(result) : NotFound(result);
    }
}
