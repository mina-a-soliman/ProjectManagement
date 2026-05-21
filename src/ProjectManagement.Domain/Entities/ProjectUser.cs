namespace ProjectManagement.Domain.Entities;

/// <summary>
/// Join entity representing the many-to-many relationship between Projects and Users.
/// </summary>
public class ProjectUser
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = default!;

    public Guid UserId { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    public Guid AssignedBy { get; set; } // AdminId
}
