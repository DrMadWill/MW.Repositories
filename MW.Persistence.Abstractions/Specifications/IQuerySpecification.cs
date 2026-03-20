namespace MW.Persistence.Abstractions.Specifications;

/// <summary>
/// Represents a specification designed for query/read scenarios.
/// Extends <see cref="ISpecification{TEntity}"/> with projection capability for query-side use cases.
/// </summary>
/// <typeparam name="TEntity">The type of the source entity.</typeparam>
/// <typeparam name="TResult">The type of the projected result.</typeparam>
public interface IQuerySpecification<TEntity, TResult> : ISpecification<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Gets the projection/selector expression to map entities into the result type.
    /// </summary>
    System.Linq.Expressions.Expression<Func<TEntity, TResult>>? Selector { get; }
}
