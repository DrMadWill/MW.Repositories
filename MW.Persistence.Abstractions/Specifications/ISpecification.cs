using System.Linq.Expressions;

namespace MW.Persistence.Abstractions.Specifications;

/// <summary>
/// Represents a specification that encapsulates reusable query logic for filtering entities.
/// Specifications help avoid repository method explosion and make query logic composable.
/// Use <see cref="Criteria"/> for database-level filtering and <see cref="IsSatisfiedBy"/> for in-memory evaluation.
/// </summary>
/// <typeparam name="TEntity">The type of the entity the specification applies to.</typeparam>
public interface ISpecification<TEntity> where TEntity : class
{
    /// <summary>
    /// Gets the filter criteria expression for database-level query filtering.
    /// </summary>
    Expression<Func<TEntity, bool>>? Criteria { get; }

    /// <summary>
    /// Gets the ordering expression for ascending sort.
    /// The <c>object</c> return type allows ordering by any property type.
    /// Implementations must handle expression translation to the underlying provider.
    /// </summary>
    Expression<Func<TEntity, object>>? OrderBy { get; }

    /// <summary>
    /// Gets the ordering expression for descending sort.
    /// The <c>object</c> return type allows ordering by any property type.
    /// Implementations must handle expression translation to the underlying provider.
    /// </summary>
    Expression<Func<TEntity, object>>? OrderByDescending { get; }

    /// <summary>
    /// Gets the number of records to skip (for paging).
    /// </summary>
    int? Skip { get; }

    /// <summary>
    /// Gets the number of records to take (for paging).
    /// </summary>
    int? Take { get; }

    /// <summary>
    /// Evaluates whether the given entity satisfies this specification in memory.
    /// This is useful for domain-level validation. For database queries, use <see cref="Criteria"/> instead.
    /// </summary>
    /// <param name="entity">The entity to evaluate.</param>
    /// <returns><c>true</c> if the entity satisfies the specification; otherwise, <c>false</c>.</returns>
    bool IsSatisfiedBy(TEntity entity);
}
