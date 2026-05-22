namespace ProjectManagement.Application.Interfaces;

/// <summary>
/// Provides access to the current authenticated user's information.
/// </summary>
public interface ICurrentUserService
{
    Guid? UserId { get; }

    bool IsAuthenticated { get; }

    bool IsInRole(string role);
}
