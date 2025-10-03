namespace AISanatanPortal.API.DTOs;

public class ApiResponse<T>
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public int? TotalCount { get; set; }
    public int? CurrentPage { get; set; }
    public int? TotalPages { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public class PaginationRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public string SortDirection { get; set; } = "asc";
    public string? SearchTerm { get; set; }
}

// Note: All model classes are now properly defined in the Models namespace
// This file contains only common DTOs and utility classes