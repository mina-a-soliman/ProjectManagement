namespace ProjectManagement.Infrastructure.Configurations.Settings;

/// <summary>
/// Swagger/OpenAPI documentation configuration settings.
/// </summary>
public sealed class SwaggerSettings
{
    public const string SectionName = "Swagger";

    public string Title { get; set; } = "Project Management API";

    public string Description { get; set; } = "API for managing projects and tasks.";

    public string Version { get; set; } = "v1";
}
