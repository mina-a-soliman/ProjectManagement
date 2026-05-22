using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ProjectManagement.Application.Interfaces;

namespace ProjectManagement.Infrastructure.Services;

/// <summary>
/// Extracts the current authenticated user's identity from the HTTP context.
/// </summary>
public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid? UserId
    {
        get
        {
            var userId = httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(userId, out var parsedId) ? parsedId : null;
        }
    }

    public bool IsInRole(string role) =>
        httpContextAccessor.HttpContext?.User.IsInRole(role) ?? false;

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
