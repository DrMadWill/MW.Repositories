namespace MW.Core.Concretes;

/// <summary>
/// Extension methods for applying <see cref="StandardFilter{TEntity}"/> to IQueryable sources.
/// </summary>
public static class StandardFilterUtils
{
    /// <summary>
    /// Applies the given <see cref="StandardFilter{TEntity}"/> to the source query.
    /// If the filter is <c>null</c>, the source is returned unchanged.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to filter.</typeparam>
    /// <param name="source">The IQueryable source to be filtered.</param>
    /// <param name="filter">The filter to apply, or <c>null</c> to skip filtering.</param>
    /// <returns>The filtered IQueryable, or the original source if filter is null.</returns>
    public static IQueryable<TEntity> FilterBy<TEntity>(this IQueryable<TEntity> source,
        StandardFilter<TEntity>? filter)
        where TEntity : class
        => filter?.ApplyFilter(source) ?? source;
}
