using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.DTOs.Projects;
using ProjectManagement.Application.DTOs.Tasks;
using ProjectManagement.Application.Features.Admin.Commands.AssignUserToProject;
using ProjectManagement.Application.Features.Admin.Commands.RemoveUserFromProject;
using ProjectManagement.Application.Features.Projects.Commands.CreateProject;
using ProjectManagement.Application.Features.Projects.Commands.DeleteProject;
using ProjectManagement.Application.Features.Projects.Commands.UpdateProject;
using ProjectManagement.Application.Features.Projects.Queries.GetAllProjects;
using ProjectManagement.Application.Features.Projects.Queries.GetMyProjects;
using ProjectManagement.Application.Features.Projects.Queries.GetProjectById;
using ProjectManagement.Application.Features.Tasks.Queries.GetTasksByProject;
using ProjectManagement.Application.Wrappers;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjectManagement.API.Controllers.v1;

/// <summary>
/// Manages project resources and memberships.
/// </summary>
[ApiVersion("1.0")]
[Authorize]
public sealed class ProjectsController(ISender mediator) : ApiControllerBase
{
    /// <summary>
    /// Creates a new project. Access is restricted to users with the Admin role.
    /// </summary>
    /// <param name="command">Project details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created project details.</returns>
    /// <response code="201">Project created successfully.</response>
    /// <response code="400">Validation failed.</response>
    /// <response code="403">Forbidden. Only administrators can create projects.</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Create Project", Description = "Creates a new project.")]
    [ProducesResponseType(typeof(Result<ProjectDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProjectCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return HandleCreatedResult(result, nameof(GetById), new { id = result.Data?.Id });
    }

    /// <summary>
    /// Retrieves all projects in the system with pagination. Access is restricted to users with the Admin role.
    /// </summary>
    /// <param name="pageNumber">The page number (default: 1).</param>
    /// <param name="pageSize">The number of records per page (default: 10).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of projects.</returns>
    /// <response code="200">Projects retrieved successfully.</response>
    /// <response code="403">Forbidden. Only administrators can view all projects.</response>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Get All Projects", Description = "Retrieves all projects in the system with pagination.")]
    [ProducesResponseType(typeof(PaginatedResponse<ProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllProjectsQuery(pageNumber, pageSize);
        var result = await mediator.Send(query, cancellationToken);
        return HandlePaginatedResult(result);
    }

    /// <summary>
    /// Retrieves projects assigned to the currently authenticated user.
    /// </summary>
    /// <param name="pageNumber">The page number (default: 1).</param>
    /// <param name="pageSize">The number of records per page (default: 10).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of the user's projects.</returns>
    /// <response code="200">Projects retrieved successfully.</response>
    [HttpGet("~/api/v{version:apiVersion}/users/me/projects")]
    [SwaggerOperation(Summary = "Get My Projects", Description = "Retrieves projects assigned to the currently authenticated user.")]
    [ProducesResponseType(typeof(PaginatedResponse<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMy(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMyProjectsQuery(pageNumber, pageSize);
        var result = await mediator.Send(query, cancellationToken);
        return HandlePaginatedResult(result);
    }

    /// <summary>
    /// Retrieves a specific project by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the project.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested project details.</returns>
    /// <response code="200">Project found.</response>
    /// <response code="404">Project not found.</response>
    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get Project by Id", Description = "Retrieves a specific project by its unique identifier.")]
    [ProducesResponseType(typeof(Result<ProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetProjectByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Updates an existing project's details.
    /// </summary>
    /// <param name="id">The unique identifier of the project to update.</param>
    /// <param name="request">The updated project properties.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated project details.</returns>
    /// <response code="200">Project updated successfully.</response>
    /// <response code="400">Validation failed.</response>
    /// <response code="404">Project not found.</response>
    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Update Project", Description = "Updates an existing project's details.")]
    [ProducesResponseType(typeof(Result<ProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProjectCommand(id, request.Name, request.Description);
        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Deletes a project and all associated tasks. Access is restricted to users with the Admin role.
    /// </summary>
    /// <param name="id">The unique identifier of the project to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A confirmation of the deletion.</returns>
    /// <response code="200">Project deleted successfully.</response>
    /// <response code="404">Project not found.</response>
    /// <response code="403">Forbidden. Only administrators can delete projects.</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Delete Project", Description = "Deletes a project and all associated tasks.")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProjectCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Assigns a user to a project. Access is restricted to users with the Admin role.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <param name="userId">The unique identifier of the user to assign.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A confirmation of the assignment.</returns>
    /// <response code="200">User assigned to project successfully.</response>
    /// <response code="400">Validation failed or user already assigned.</response>
    /// <response code="404">Project or user not found.</response>
    /// <response code="403">Forbidden. Only administrators can assign users to projects.</response>
    [HttpPost("{projectId:guid}/members/{userId:guid}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Assign User to Project", Description = "Assigns a user to a project (members collection).")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AssignMember(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new AssignUserToProjectCommand(projectId, userId);
        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Removes a user from a project. Access is restricted to users with the Admin role.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <param name="userId">The unique identifier of the user to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A confirmation of the removal.</returns>
    /// <response code="200">User removed from project successfully.</response>
    /// <response code="400">Validation failed or user not member of project.</response>
    /// <response code="404">Project or user not found.</response>
    /// <response code="403">Forbidden. Only administrators can remove users from projects.</response>
    [HttpDelete("{projectId:guid}/members/{userId:guid}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Remove User from Project", Description = "Removes a user from a project (members collection).")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveMember(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveUserFromProjectCommand(projectId, userId);
        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves all tasks for a specific project with pagination.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <param name="pageNumber">The page number (default: 1).</param>
    /// <param name="pageSize">The number of records per page (default: 10).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of tasks for the project.</returns>
    /// <response code="200">Tasks retrieved successfully.</response>
    /// <response code="404">Project not found.</response>
    [HttpGet("{projectId:guid}/tasks")]
    [SwaggerOperation(Summary = "Get Project Tasks", Description = "Retrieves all tasks for a specific project with pagination.")]
    [ProducesResponseType(typeof(PaginatedResponse<TaskDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProjectTasks(
        Guid projectId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTasksByProjectQuery(projectId, pageNumber, pageSize);
        var result = await mediator.Send(query, cancellationToken);
        return HandlePaginatedResult(result);
    }
}
