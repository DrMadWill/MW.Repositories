using System.Linq.Expressions;
using MW.Persistence.Abstractions.Specifications;

namespace MW.Persistence.EntityFrameworkCore.Specifications;

/// <summary>
/// Base implementation of the specification pattern.
/// Encapsulates reusable, composable query logic for filtering, ordering, and paging entities.
/// Inherit from this class to create domain-specific specifications.
/// </summary>
/// <typeparam name="TEntity">The type of the entity the specification applies to.</typeparam>
public abstract class BaseSpecification<TEntity> : ISpecification<TEntity>
    where TEntity : class
{
    /// <inheritdoc />
    public Expression<Func<TEntity, bool>>? Criteria { get; private set; }

    /// <inheritdoc />
    public Expression<Func<TEntity, object>>? OrderBy { get; private set; }

    /// <inheritdoc />
    public Expression<Func<TEntity, object>>? OrderByDescending { get; private set; }

    /// <inheritdoc />
    public int? Skip { get; private set; }

    /// <inheritdoc />
    public int? Take { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseSpecification{TEntity}"/> class with no criteria.
    /// </summary>
    protected BaseSpecification()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseSpecification{TEntity}"/> class with the specified filter criteria.
    /// </summary>
    /// <param name="criteria">The filter expression to apply.</param>
    protected BaseSpecification(Expression<Func<TEntity, bool>> criteria)
    {
        Criteria = criteria;
    }

    /// <inheritdoc />
    public virtual bool IsSatisfiedBy(TEntity entity)
    {
        return Criteria is null || Criteria.Compile()(entity);
    }

    /// <summary>
    /// Sets the ascending order expression.
    /// </summary>
    /// <param name="orderByExpression">The ordering expression.</param>
    protected void ApplyOrderBy(Expression<Func<TEntity, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    /// <summary>
    /// Sets the descending order expression.
    /// </summary>
    /// <param name="orderByDescendingExpression">The ordering expression.</param>
    protected void ApplyOrderByDescending(Expression<Func<TEntity, object>> orderByDescendingExpression)
    {
        OrderByDescending = orderByDescendingExpression;
    }

    /// <summary>
    /// Sets the paging parameters.
    /// </summary>
    /// <param name="skip">The number of records to skip.</param>
    /// <param name="take">The number of records to take.</param>
    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
    }
}
