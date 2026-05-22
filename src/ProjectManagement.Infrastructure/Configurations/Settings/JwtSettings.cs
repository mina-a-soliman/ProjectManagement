namespace ProjectManagement.Infrastructure.Configurations.Settings;

/// <summary>
/// JWT authentication configuration settings.
/// </summary>
public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = default!;

    public string Audience { get; set; } = default!;

    public string SecretKey { get; set; } = default!;

    public int ExpiryMinutes { get; set; } = 60;

    public int RefreshTokenExpiryDays { get; set; } = 7;
}
