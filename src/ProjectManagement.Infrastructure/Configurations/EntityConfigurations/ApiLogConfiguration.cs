using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Configurations.EntityConfigurations;

/// <summary>
/// EF Core Fluent API configuration for the ApiLog entity.
/// </summary>
public sealed class ApiLogConfiguration : IEntityTypeConfiguration<ApiLog>
{
    public void Configure(EntityTypeBuilder<ApiLog> builder)
    {
        builder.ToTable("ApiLogs");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Method)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(l => l.Url)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(l => l.Request)
            .HasColumnType("nvarchar(max)");

        builder.Property(l => l.Response)
            .HasColumnType("nvarchar(max)");

        builder.Property(l => l.Exception)
            .HasColumnType("nvarchar(max)");

        builder.Property(l => l.CorrelationId)
            .HasMaxLength(50);

        builder.Property(l => l.CreatedAt)
            .IsRequired();

        // Indexes for querying logs
        builder.HasIndex(l => l.CreatedAt)
            .HasDatabaseName("IX_ApiLogs_CreatedAt");

        builder.HasIndex(l => l.UserId)
            .HasDatabaseName("IX_ApiLogs_UserId");

        builder.HasIndex(l => l.StatusCode)
            .HasDatabaseName("IX_ApiLogs_StatusCode");
    }
}
