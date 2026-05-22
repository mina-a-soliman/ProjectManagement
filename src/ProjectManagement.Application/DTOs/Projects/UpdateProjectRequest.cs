namespace ProjectManagement.Application.DTOs.Projects;

/// <summary>
/// Request model for updating a project.
/// </summary>
public sealed record UpdateProjectRequest(
    string Name,
    string? Description);
