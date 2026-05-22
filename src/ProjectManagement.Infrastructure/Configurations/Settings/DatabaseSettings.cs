namespace ProjectManagement.Infrastructure.Configurations.Settings;

/// <summary>
/// Database connection configuration settings.
/// </summary>
public sealed class DatabaseSettings
{
    public const string SectionName = "Database";

    public string ConnectionString { get; set; } = default!;
}
