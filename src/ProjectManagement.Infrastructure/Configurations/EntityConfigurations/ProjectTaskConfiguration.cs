using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Configurations.EntityConfigurations;

/// <summary>
/// EF Core Fluent API configuration for the ProjectTask entity.
/// </summary>
public sealed class ProjectTaskConfiguration : IEntityTypeConfiguration<ProjectTask>
{
    public void Configure(EntityTypeBuilder<ProjectTask> builder)
    {
        builder.ToTable("ProjectTasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(t => t.Priority)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(t => t.ProjectId)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        // Index for efficient project-scoped queries
        builder.HasIndex(t => t.ProjectId)
            .HasDatabaseName("IX_ProjectTasks_ProjectId");

        // Index for efficient assignment-scoped queries
        builder.HasIndex(t => t.AssignedUserId)
            .HasDatabaseName("IX_ProjectTasks_AssignedUserId");
    }
}
