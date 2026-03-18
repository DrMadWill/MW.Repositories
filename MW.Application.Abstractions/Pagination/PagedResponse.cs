namespace MW.Application.Abstractions.Pagination;

/// <summary>
/// Represents a paginated response containing a subset of items and pagination metadata.
/// </summary>
/// <typeparam name="T">The type of items in the response.</typeparam>
public class PagedResponse<T>
{
    /// <summary>
    /// Gets or sets the collection of items for the current page.
    /// </summary>
    public IReadOnlyCollection<T> Items { get; set; } = Array.Empty<T>();

    /// <summary>
    /// Gets or sets the total number of items across all pages.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the current page number (1-based).
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    public int PageSize { get; set; }
}
