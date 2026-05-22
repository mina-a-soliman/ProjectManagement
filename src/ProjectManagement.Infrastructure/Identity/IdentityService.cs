
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using ProjectManagement.Application.Auth.DTOs;
using ProjectManagement.Application.DTOs.Auth;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Application.Wrappers;
using ProjectManagement.Infrastructure.Configurations.Settings;

namespace ProjectManagement.Infrastructure.Identity;

/// <summary>
/// Implements user registration and login using ASP.NET Identity and JWT token generation.
/// </summary>
public sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    IJwtTokenGenerator jwtTokenGenerator,
    IOptions<JwtSettings> jwtOptions) : IIdentityService
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public async Task<Result<AuthResponse>> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        // Check if user already exists
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
            return Result<AuthResponse>.FailureResponse("A user with this email already exists.");

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = request.Email,
            UserName = request.Email
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return Result<AuthResponse>.FailureResponse("Registration failed.", errors);
        }

        // Assign default User role
        await userManager.AddToRoleAsync(user, "User");

        var roles = await userManager.GetRolesAsync(user);
        var accessToken = jwtTokenGenerator.GenerateAccessToken(user.Id, user.Email!, roles);
        var refreshToken = jwtTokenGenerator.GenerateRefreshToken();

        // Store refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);
        await userManager.UpdateAsync(user);

        var authResponse = new AuthResponse(
            user.Id,
            user.Email!,
            user.FullName,
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes));

        return Result<AuthResponse>.SuccessResponse(authResponse, "Registration successful.");
    }

    public async Task<Result<AuthResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return Result<AuthResponse>.FailureResponse("Invalid email or password.");

        var isPasswordValid = await userManager.CheckPasswordAsync(user, request.Password);

        if (!isPasswordValid)
            return Result<AuthResponse>.FailureResponse("Invalid email or password.");

        var roles = await userManager.GetRolesAsync(user);
        var accessToken = jwtTokenGenerator.GenerateAccessToken(user.Id, user.Email!, roles);
        var refreshToken = jwtTokenGenerator.GenerateRefreshToken();

        // Update refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);
        await userManager.UpdateAsync(user);

        var authResponse = new AuthResponse(
            user.Id,
            user.Email!,
            user.FullName,
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes));

        return Result<AuthResponse>.SuccessResponse(authResponse, "Login successful.");
    }

    public async Task<Result<Guid>> CreateUserAsync(
        string email,
        string password,
        string fullName,
        List<string> roles,
        CancellationToken cancellationToken = default)
    {
        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser is not null)
            return Result<Guid>.FailureResponse("A user with this email already exists.");

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FullName = fullName,
            Email = email,
            UserName = email
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return Result<Guid>.FailureResponse("User creation failed.", errors);
        }

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = role });
            }
            await userManager.AddToRoleAsync(user, role);
        }

        return Result<Guid>.SuccessResponse(user.Id, "User created successfully.");
    }

    public async Task<Result<bool>> CreateRoleAsync(
        string roleName,
        CancellationToken cancellationToken = default)
    {
        if (await roleManager.RoleExistsAsync(roleName))
            return Result<bool>.FailureResponse("Role already exists.");

        var result = await roleManager.CreateAsync(new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = roleName });
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return Result<bool>.FailureResponse("Role creation failed.", errors);
        }

        return Result<bool>.SuccessResponse(true, "Role created successfully.");
    }

    public async Task<Result<bool>> AddUserToRoleAsync(
        Guid userId,
        string role,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result<bool>.FailureResponse("User not found.");

        if (!await roleManager.RoleExistsAsync(role))
            return Result<bool>.FailureResponse("Role does not exist.");

        if (await userManager.IsInRoleAsync(user, role))
            return Result<bool>.FailureResponse("User is already in this role.");

        var result = await userManager.AddToRoleAsync(user, role);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return Result<bool>.FailureResponse("Failed to assign role to user.", errors);
        }

        return Result<bool>.SuccessResponse(true, "Role assigned successfully.");
    }

    public async Task<Result<bool>> RemoveUserFromRoleAsync(
        Guid userId,
        string role,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result<bool>.FailureResponse("User not found.");

        if (!await userManager.IsInRoleAsync(user, role))
            return Result<bool>.FailureResponse("User is not in this role.");

        var result = await userManager.RemoveFromRoleAsync(user, role);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return Result<bool>.FailureResponse("Failed to remove role from user.", errors);
        }

        return Result<bool>.SuccessResponse(true, "Role removed successfully.");
    }

    public async Task<Result<List<string>>> GetUserRolesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result<List<string>>.FailureResponse("User not found.");

        var roles = await userManager.GetRolesAsync(user);
        return Result<List<string>>.SuccessResponse(roles.ToList(), "Roles retrieved successfully.");
    }

    public async Task<bool> UserExistsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        return user is not null;
    }
}

