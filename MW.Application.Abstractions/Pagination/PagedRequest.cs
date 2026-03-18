namespace MW.Application.Abstractions.Pagination;

/// <summary>
/// Represents a standard paginated request with page number and page size.
/// Used by query handlers to return paginated results.
/// </summary>
public class PagedRequest
{
    /// <summary>
    /// Gets or sets the page number (1-based).
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    public int PageSize { get; set; } = 20;
}
