using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.DTOs.Auth;
using ProjectManagement.Application.Features.Auth.Commands.Login;
using ProjectManagement.Application.Features.Auth.Commands.Register;
using ProjectManagement.Application.Wrappers;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjectManagement.API.Controllers.v1;

/// <summary>
/// Handles user authentication operations including registration and login.
/// </summary>
[ApiVersion("1.0")]
public sealed class AuthController(ISender mediator) : ApiControllerBase
{
    /// <summary>
    /// Registers a new user account (restricted to Admin users).
    /// </summary>
    /// <param name="request">Registration details including name, email, and password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Authentication response with JWT tokens.</returns>
    /// <response code="200">Registration successful.</response>
    /// <response code="400">Validation failed or user already exists.</response>
    /// <response code="401">Unauthorized. Invalid or missing authentication credentials.</response>
    /// <response code="403">Forbidden. Only administrators can register new users.</response>
    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Register User", Description = "Registers a new user account (restricted to Admin users).")]
    [ProducesResponseType(typeof(Result<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Authenticates a user and returns access and refresh JWT tokens.
    /// </summary>
    /// <param name="request">Login credentials (email and password).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Authentication response with JWT tokens.</returns>
    /// <response code="200">Login successful.</response>
    /// <response code="400">Invalid credentials or validation failed.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Login User", Description = "Authenticates a user and returns access and refresh JWT tokens.")]
    [ProducesResponseType(typeof(Result<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return HandleResult(result);
    }
}
