using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Infrastructure.Identity;

namespace ProjectManagement.Infrastructure.Configurations.EntityConfigurations;

/// <summary>
/// EF Core Fluent API configuration for the ProjectUser join entity.
/// </summary>
public sealed class ProjectUserConfiguration : IEntityTypeConfiguration<ProjectUser>
{
    public void Configure(EntityTypeBuilder<ProjectUser> builder)
    {
        builder.ToTable("ProjectUsers");

        builder.HasKey(pu => new { pu.ProjectId, pu.UserId });

        builder.HasOne(pu => pu.Project)
            .WithMany(p => p.ProjectUsers)
            .HasForeignKey(pu => pu.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
            .WithMany(u => u.ProjectUsers)
            .HasForeignKey(pu => pu.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(pu => pu.AssignedAt)
            .IsRequired();

        builder.Property(pu => pu.AssignedBy)
            .IsRequired();

        // Indexes for efficient querying
        builder.HasIndex(pu => pu.ProjectId)
            .HasDatabaseName("IX_ProjectUsers_ProjectId");

        builder.HasIndex(pu => pu.UserId)
            .HasDatabaseName("IX_ProjectUsers_UserId");
    }
}
