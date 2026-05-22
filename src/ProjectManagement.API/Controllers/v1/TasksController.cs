using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.DTOs.Tasks;
using ProjectManagement.Application.Features.Admin.Commands.AssignTaskToUser;
using ProjectManagement.Application.Features.Tasks.Commands.CreateTask;
using ProjectManagement.Application.Features.Tasks.Commands.DeleteTask;
using ProjectManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;
using ProjectManagement.Application.Features.Tasks.Queries.GetMyTasks;
using ProjectManagement.Application.Wrappers;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjectManagement.API.Controllers.v1;

/// <summary>
/// Manages task resources, status updates, and assignments.
/// </summary>
[ApiVersion("1.0")]
[Authorize]
public sealed class TasksController(ISender mediator) : ApiControllerBase
{
    /// <summary>
    /// Creates a new task. Access is restricted to users with the Admin role.
    /// </summary>
    /// <param name="command">Task creation details including the target project.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created task details.</returns>
    /// <response code="201">Task created successfully.</response>
    /// <response code="400">Validation failed or project not found.</response>
    /// <response code="403">Forbidden. Only administrators can create tasks.</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Create Task", Description = "Creates a new task.")]
    [ProducesResponseType(typeof(Result<TaskDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] CreateTaskCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return HandleCreatedResult(result, nameof(GetMy), new { });
    }

    /// <summary>
    /// Retrieves tasks assigned to the currently authenticated user with pagination.
    /// </summary>
    /// <param name="pageNumber">The page number (default: 1).</param>
    /// <param name="pageSize">The number of records per page (default: 10).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of tasks assigned to the user.</returns>
    /// <response code="200">Tasks retrieved successfully.</response>
    [HttpGet("~/api/v{version:apiVersion}/users/me/tasks")]
    [SwaggerOperation(Summary = "Get My Tasks", Description = "Retrieves tasks assigned to the currently authenticated user with pagination.")]
    [ProducesResponseType(typeof(PaginatedResponse<TaskDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMy(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMyTasksQuery(pageNumber, pageSize);
        var result = await mediator.Send(query, cancellationToken);
        return HandlePaginatedResult(result);
    }

    /// <summary>
    /// Assigns or replaces the assignee of a specific task. Access is restricted to users with the Admin role.
    /// </summary>
    /// <param name="taskId">The unique identifier of the task.</param>
    /// <param name="userId">The unique identifier of the user to assign.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A confirmation of the assignment.</returns>
    /// <response code="200">Task assignee updated successfully.</response>
    /// <response code="400">Validation failed.</response>
    /// <response code="404">Task or user not found.</response>
    /// <response code="403">Forbidden. Only administrators can assign tasks.</response>
    [HttpPut("{taskId:guid}/assignee/{userId:guid}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Assign Task Assignee", Description = "Assigns or replaces the assignee of a specific task.")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AssignAssignee(
        Guid taskId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new AssignTaskToUserCommand(taskId, userId);
        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Updates the execution status of a task.
    /// </summary>
    /// <param name="id">The unique identifier of the task.</param>
    /// <param name="request">The new status update details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated task details.</returns>
    /// <response code="200">Task status updated successfully.</response>
    /// <response code="400">Validation failed or unauthorized to edit task.</response>
    /// <response code="404">Task not found.</response>
    [HttpPatch("{id:guid}/status")]
    [SwaggerOperation(Summary = "Update Task Status", Description = "Updates the execution status of a task.")]
    [ProducesResponseType(typeof(Result<TaskDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateTaskStatusRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTaskStatusCommand(id, request.Status);
        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Deletes a task. Access is restricted to users with the Admin role.
    /// </summary>
    /// <param name="id">The unique identifier of the task to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A confirmation of deletion.</returns>
    /// <response code="200">Task deleted successfully.</response>
    /// <response code="404">Task not found.</response>
    /// <response code="403">Forbidden. Only administrators can delete tasks.</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Delete Task", Description = "Deletes a task.")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteTaskCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
