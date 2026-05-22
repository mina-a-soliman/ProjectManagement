namespace ProjectManagement.Application.DTOs.Projects;

/// <summary>
/// Request model for creating a project.
/// </summary>
public sealed record CreateProjectRequest(
    string Name,
    string? Description);
