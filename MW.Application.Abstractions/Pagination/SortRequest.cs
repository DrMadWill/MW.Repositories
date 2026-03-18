namespace MW.Application.Abstractions.Pagination;

/// <summary>
/// Specifies the direction of sorting.
/// </summary>
public enum SortDirection
{
    /// <summary>
    /// Sort in ascending order.
    /// </summary>
    Ascending,

    /// <summary>
    /// Sort in descending order.
    /// </summary>
    Descending
}

/// <summary>
/// Represents a sorting request with field name and direction.
/// Used by query handlers to apply sorting to results.
/// </summary>
public class SortRequest
{
    /// <summary>
    /// Gets or sets the name of the field to sort by.
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Gets or sets the sort direction.
    /// </summary>
    public SortDirection SortDirection { get; set; } = SortDirection.Ascending;
}
