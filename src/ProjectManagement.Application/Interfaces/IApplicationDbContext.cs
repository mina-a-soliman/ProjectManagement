using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Interfaces;


/// <summary>
/// Abstraction over the EF Core DbContext for the application layer.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Project> Projects { get; }
    DbSet<ProjectTask> ProjectTasks { get; }
    DbSet<ApiLog> ApiLogs { get; }
    DbSet<ProjectUser> ProjectUsers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
