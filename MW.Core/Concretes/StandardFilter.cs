namespace MW.Core.Concretes;

/// <summary>
/// Abstract base class for creating reusable, composable IQueryable filters.
/// Implement <see cref="ApplyFilter"/> to define filtering logic for a specific entity type.
/// </summary>
/// <typeparam name="TEntity">The entity type to filter.</typeparam>
public abstract class StandardFilter<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Applies the filter to the given <see cref="IQueryable{TEntity}"/> source.
    /// </summary>
    /// <param name="source">The IQueryable source to be filtered.</param>
    /// <returns>The filtered IQueryable.</returns>
    public abstract IQueryable<TEntity> ApplyFilter(IQueryable<TEntity> source);
}
