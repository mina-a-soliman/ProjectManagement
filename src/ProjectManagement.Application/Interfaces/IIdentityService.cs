using ProjectManagement.Application.Auth.DTOs;
using ProjectManagement.Application.DTOs.Auth;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.Application.Interfaces;

/// <summary>
/// Abstraction over ASP.NET Identity operations.
/// </summary>
public interface IIdentityService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task<Result<Guid>> CreateUserAsync(string email, string password, string fullName, List<string> roles, CancellationToken cancellationToken = default);

    Task<Result<bool>> CreateRoleAsync(string roleName, CancellationToken cancellationToken = default);

    Task<Result<bool>> AddUserToRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default);

    Task<Result<bool>> RemoveUserFromRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default);

    Task<Result<List<string>>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken = default);
}
