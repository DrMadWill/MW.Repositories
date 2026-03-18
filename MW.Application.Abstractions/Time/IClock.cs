namespace MW.Application.Abstractions.Time;

/// <summary>
/// Abstraction for accessing the current time.
/// Application services use this interface instead of calling <see cref="DateTimeOffset.UtcNow"/> directly.
/// Enables deterministic testing and consistent time handling.
/// </summary>
public interface IClock
{
    /// <summary>
    /// Gets the current date and time with timezone information.
    /// </summary>
    DateTimeOffset Now { get; }
}
