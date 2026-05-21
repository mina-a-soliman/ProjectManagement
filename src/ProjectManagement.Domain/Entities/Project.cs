using ProjectManagement.Domain.Common;

namespace ProjectManagement.Domain.Entities;

/// <summary>
/// Represents a project owned by a user.
/// </summary>
public class Project : BaseEntity
{
    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    private readonly List<ProjectTask> _tasks = [];

    /// <summary>
    /// Tasks belonging to this project. Read-only collection to enforce encapsulation.
    /// </summary>
    public IReadOnlyCollection<ProjectTask> Tasks => _tasks.AsReadOnly();

    private readonly List<ProjectUser> _projectUsers = [];

    /// <summary>
    /// Users assigned to this project. Read-only collection to enforce encapsulation.
    /// </summary>
    public IReadOnlyCollection<ProjectUser> ProjectUsers => _projectUsers.AsReadOnly();

    /// <summary>
    /// Factory method to create a new project with required fields.
    /// </summary>
    public static Project Create(string name, string? description, Guid createdBy)
    {
        return new Project
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    /// <summary>
    /// Updates the project's mutable fields.
    /// </summary>
    public void Update(string name, string? description, Guid updatedBy)
    {
        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
