using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.Wrappers;

namespace ProjectManagement.API.Controllers;

/// <summary>
/// Base class for all API Controllers. Provides helper methods for formatting responses with standardized status codes.
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Processes a standard result and returns the appropriate Ok or error response.
    /// </summary>
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result is null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                Result.FailureResponse("Internal Server Error: Command result was null."));
        }

        if (!result.Success)
        {
            var statusCode = InferStatusCode(result.Message);
            return StatusCode(statusCode, result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Processes a paginated result and returns the PaginatedResponse directly.
    /// </summary>
    protected IActionResult HandlePaginatedResult<T>(PaginatedResponse<T> result)
    {
        if (result is null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                Result.FailureResponse("Internal Server Error: Query result was null."));
        }

        if (!result.Success)
        {
            var statusCode = InferStatusCode(result.Message);
            return StatusCode(statusCode, result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Processes a creation result and returns a 201 Created response.
    /// </summary>
    protected IActionResult HandleCreatedResult<T>(Result<T> result, string actionName, object routeValues)
    {
        if (result is null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                Result.FailureResponse("Internal Server Error: Command result was null."));
        }

        if (!result.Success)
        {
            var statusCode = InferStatusCode(result.Message);
            return StatusCode(statusCode, result);
        }

        return CreatedAtAction(actionName, routeValues, result);
    }

    private int InferStatusCode(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return StatusCodes.Status400BadRequest;

        var lowerMessage = message.ToLowerInvariant();
        
        if (lowerMessage.Contains("not found"))
            return StatusCodes.Status404NotFound;
        
        if (lowerMessage.Contains("unauthorized") || lowerMessage.Contains("forbidden") || lowerMessage.Contains("only administrators"))
            return StatusCodes.Status403Forbidden;

        return StatusCodes.Status400BadRequest;
    }
}
