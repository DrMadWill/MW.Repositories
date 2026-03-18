namespace MW.Core.Abstractions;

/// <summary>
/// Represents an entity that supports optimistic concurrency.
/// Compatible with EF Core concurrency token.
/// </summary>
public interface IHasConcurrencyToken
{
    /// <summary>
    /// Gets or sets the concurrency stamp used for optimistic concurrency control.
    /// </summary>
    string ConcurrencyStamp { get; set; }
}
