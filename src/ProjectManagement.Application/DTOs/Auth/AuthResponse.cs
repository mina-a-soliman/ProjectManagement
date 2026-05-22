namespace ProjectManagement.Application.DTOs.Auth;


/// <summary>
/// Authentication response containing tokens and user information.
/// </summary>
public sealed record AuthResponse(
    Guid UserId,
    string Email,
    string FullName,
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiry);
