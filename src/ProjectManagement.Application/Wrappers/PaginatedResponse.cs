namespace ProjectManagement.Application.Wrappers;

/// <summary>
/// Paginated response wrapper with pagination metadata.
/// </summary>
public class PaginatedResponse<T> : Result<IReadOnlyList<T>>
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

    public static PaginatedResponse<T> Create(
        IReadOnlyList<T> data,
        int totalCount,
        int pageNumber,
        int pageSize,
        string message = "Data retrieved successfully")
    {
        return new PaginatedResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
