using Microsoft.AspNetCore.Identity;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Identity;

/// <summary>
/// Application user extending ASP.NET Identity with additional profile fields.
/// </summary>
public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = default!;

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();
}
