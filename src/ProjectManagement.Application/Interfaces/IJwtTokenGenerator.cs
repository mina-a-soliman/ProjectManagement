namespace ProjectManagement.Application.Interfaces;

/// <summary>
/// Generates JWT access tokens and refresh tokens.
/// </summary>
public interface IJwtTokenGenerator
{
    string GenerateAccessToken(Guid userId, string email, IList<string> roles);

    string GenerateRefreshToken();
}
