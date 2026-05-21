using ProjectManagement.Domain.Common;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Domain.Entities;


/// <summary>
/// Represents a task within a project.
/// Named ProjectTask to avoid conflict with System.Threading.Tasks.Task.
/// </summary>
public class ProjectTask : BaseEntity
{
    public string Title { get; set; } = default!;

    public string? Description { get; set; }

    public ProjectTaskStatus Status { get; set; } = ProjectTaskStatus.ToDo;

    public DateTime? DueDate { get; set; }

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public Guid ProjectId { get; set; }

    /// <summary>
    /// Navigation property to the parent project.
    /// </summary>
    public Project Project { get; set; } = default!;

    /// <summary>
    /// The ID of the user assigned to this task.
    /// </summary>
    public Guid? AssignedUserId { get; set; }

    /// <summary>
    /// Factory method to create a new task with required fields.
    /// </summary>
    public static ProjectTask Create(
        string title,
        string? description,
        ProjectTaskStatus status,
        DateTime? dueDate,
        TaskPriority priority,
        Guid projectId,
        Guid createdBy,
        Guid? assignedUserId = null)
    {
        return new ProjectTask
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            Status = status,
            DueDate = dueDate,
            Priority = priority,
            ProjectId = projectId,
            AssignedUserId = assignedUserId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    /// <summary>
    /// Assigns or unassigns a user to/from the task.
    /// </summary>
    public void AssignUser(Guid? assignedUserId, Guid updatedBy)
    {
        AssignedUserId = assignedUserId;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Updates the task status.
    /// </summary>
    public void UpdateStatus(ProjectTaskStatus status, Guid updatedBy)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
