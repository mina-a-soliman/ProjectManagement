using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.DTOs.Projects;
using ProjectManagement.Application.Features.Projects.Commands.CreateProject;
using ProjectManagement.Application.Features.Projects.Commands.DeleteProject;
using ProjectManagement.Application.Features.Projects.Commands.UpdateProject;
using ProjectManagement.Application.Features.Projects.Queries.GetAllProjects;
using ProjectManagement.Application.Features.Projects.Queries.GetMyProjects;
using ProjectManagement.Application.Features.Projects.Queries.GetProjectById;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.API.Controllers.v1;

/// <summary>
/// Manages project CRUD operations. All endpoints require authentication.
/// Users can only access their own projects.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
[Produces("application/json")]
[Authorize]
public sealed class ProjectsController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="command">Project creation details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created project.</returns>
    /// <response code="201">Project created successfully.</response>
    /// <response code="400">Validation failed.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<ProjectDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<ProjectDto>), StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        [FromBody] CreateProjectCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        return result.Success
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result)
            : BadRequest(result);
    }

    /// <summary>
    /// Retrieves all projects for the authenticated user with pagination.
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Page size (default: 10).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of projects.</returns>
    /// <response code="200">Projects retrieved successfully.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllProjectsQuery(pageNumber, pageSize);
        var result = await mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves all projects assigned to the authenticated user with pagination.
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Page size (default: 10).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of assigned projects.</returns>
    /// <response code="200">Projects retrieved successfully.</response>
    [HttpGet("my")]
    [ProducesResponseType(typeof(PaginatedResponse<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMy(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMyProjectsQuery(pageNumber, pageSize);
        var result = await mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves a specific project by its Id.
    /// </summary>
    /// <param name="id">Project Id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested project.</returns>
    /// <response code="200">Project found.</response>
    /// <response code="404">Project not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Result<ProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<ProjectDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetProjectByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    /// <param name="id">Project Id.</param>
    /// <param name="request">Updated project details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated project.</returns>
    /// <response code="200">Project updated successfully.</response>
    /// <response code="404">Project not found.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Result<ProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<ProjectDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProjectCommand(id, request.Name, request.Description);
        var result = await mediator.Send(command, cancellationToken);

        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Deletes a project and all its tasks.
    /// </summary>
    /// <param name="id">Project Id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Deletion confirmation.</returns>
    /// <response code="200">Project deleted successfully.</response>
    /// <response code="404">Project not found.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProjectCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        return result.Success ? Ok(result) : NotFound(result);
    }
}
