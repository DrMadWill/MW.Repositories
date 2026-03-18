namespace MW.Application.Abstractions.Context;

/// <summary>
/// Abstraction for accessing the current authenticated user.
/// Application services use this interface instead of depending on HTTP context directly.
/// Infrastructure layer provides the implementation.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Gets the unique identifier of the authenticated user.
    /// Returns <c>null</c> if the user is not authenticated.
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Gets the username of the authenticated user.
    /// Returns <c>null</c> if the user is not authenticated.
    /// </summary>
    string? UserName { get; }

    /// <summary>
    /// Gets a value indicating whether the current user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the roles assigned to the current user.
    /// </summary>
    IReadOnlyCollection<string> Roles { get; }
}
