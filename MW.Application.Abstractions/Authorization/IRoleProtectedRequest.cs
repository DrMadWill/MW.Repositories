namespace MW.Application.Abstractions.Authorization;

/// <summary>
/// Marker interface for requests that require specific roles.
/// Pipeline behaviors can inspect this marker to enforce role-based authorization.
/// </summary>
public interface IRoleProtectedRequest
{
    /// <summary>
    /// Gets the collection of roles required to execute this request.
    /// </summary>
    IEnumerable<string> RequiredRoles { get; }
}
