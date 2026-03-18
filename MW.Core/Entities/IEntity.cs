namespace MW.Core.Entities;

/// <summary>
/// Represents the base identity contract for all entities.
/// </summary>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public interface IEntity<TId>
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    TId Id { get; set; }
}
