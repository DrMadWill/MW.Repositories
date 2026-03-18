using MW.Core.Entities;

namespace MW.Core.Models.Abstractions;

/// <summary>
/// Represents the base identity contract for entities.
/// Extends <see cref="IEntity{TId}"/> for backward compatibility.
/// </summary>
/// <typeparam name="T">The type of the Id property.</typeparam>
public interface IOriginEntity<T> : IEntity<T>
{
}