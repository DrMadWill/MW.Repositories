using System.Linq.Expressions;
using MW.Persistence.Abstractions.Specifications;

namespace MW.Persistence.EntityFrameworkCore.Specifications;

/// <summary>
/// Base implementation of a query specification with projection support.
/// Extends <see cref="BaseSpecification{TEntity}"/> with a selector expression for query-side use cases.
/// </summary>
/// <typeparam name="TEntity">The type of the source entity.</typeparam>
/// <typeparam name="TResult">The type of the projected result.</typeparam>
public abstract class QuerySpecification<TEntity, TResult> : BaseSpecification<TEntity>, IQuerySpecification<TEntity, TResult>
    where TEntity : class
{
    /// <inheritdoc />
    public Expression<Func<TEntity, TResult>>? Selector { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QuerySpecification{TEntity, TResult}"/> class with no criteria.
    /// </summary>
    protected QuerySpecification()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QuerySpecification{TEntity, TResult}"/> class with the specified filter criteria.
    /// </summary>
    /// <param name="criteria">The filter expression to apply.</param>
    protected QuerySpecification(Expression<Func<TEntity, bool>> criteria) : base(criteria)
    {
    }

    /// <summary>
    /// Sets the projection/selector expression.
    /// </summary>
    /// <param name="selector">The projection expression to map entities into the result type.</param>
    protected void ApplySelector(Expression<Func<TEntity, TResult>> selector)
    {
        Selector = selector;
    }
}
