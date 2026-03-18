namespace MW.Application.Abstractions.Caching;

/// <summary>
/// Marker interface for queries that support caching.
/// Pipeline behaviors can inspect this marker to implement caching strategies.
/// Infrastructure layer handles the actual caching implementation.
/// </summary>
public interface ICacheableQuery
{
    /// <summary>
    /// Gets the cache key used to store and retrieve the cached result.
    /// </summary>
    string CacheKey { get; }

    /// <summary>
    /// Gets the optional cache expiration duration.
    /// When <c>null</c>, the default cache expiration policy is used.
    /// </summary>
    TimeSpan? Expiration { get; }
}
