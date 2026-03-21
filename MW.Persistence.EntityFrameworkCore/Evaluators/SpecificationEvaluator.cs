using Microsoft.EntityFrameworkCore;
using MW.Persistence.Abstractions.Specifications;

namespace MW.Persistence.EntityFrameworkCore.Evaluators;

/// <summary>
/// Evaluates <see cref="ISpecification{TEntity}"/> instances against an <see cref="IQueryable{TEntity}"/> source.
/// Applies criteria filtering, ordering, and paging in a consistent and reusable manner.
/// </summary>
public static class SpecificationEvaluator
{
    /// <summary>
    /// Applies the given specification to the queryable source.
    /// Includes criteria filtering, ordering (ascending or descending), and paging (skip/take).
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="source">The queryable source to apply the specification to.</param>
    /// <param name="specification">The specification containing query logic.</param>
    /// <returns>The filtered and configured queryable.</returns>
    public static IQueryable<TEntity> GetQuery<TEntity>(
        IQueryable<TEntity> source,
        ISpecification<TEntity> specification) where TEntity : class
    {
        var query = source;

        if (specification.Criteria is not null)
            query = query.Where(specification.Criteria);

        if (specification.OrderBy is not null)
            query = query.OrderBy(specification.OrderBy);
        else if (specification.OrderByDescending is not null)
            query = query.OrderByDescending(specification.OrderByDescending);

        if (specification.Skip.HasValue)
            query = query.Skip(specification.Skip.Value);

        if (specification.Take.HasValue)
            query = query.Take(specification.Take.Value);

        return query;
    }
}
