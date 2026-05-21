using System.Text.Json.Serialization;

namespace ProjectManagement.Application.Wrappers;


/// <summary>
/// Standardized API response wrapper.
/// </summary>
public class Result<T>
{
    public bool Success { get; set; }

    public string Message { get; set; } = default!;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Errors { get; set; }

    public static Result<T> SuccessResponse(T data, string message = "Operation completed successfully")
    {
        return new Result<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static Result<T> FailureResponse(string message, List<string>? errors = null)
    {
        return new Result<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}

/// <summary>
/// Non-generic version for responses without data payload.
/// </summary>
public class Result : Result<object>
{
    public static Result SuccessResponse(string message = "Operation completed successfully")
    {
        return new Result
        {
            Success = true,
            Message = message
        };
    }

    public new static Result FailureResponse(string message, List<string>? errors = null)
    {
        return new Result
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}
