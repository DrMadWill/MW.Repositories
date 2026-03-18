namespace MW.Application.Abstractions.Authorization;

/// <summary>
/// Marker interface for requests that require specific permissions.
/// Pipeline behaviors can inspect this marker to enforce permission-based authorization.
/// </summary>
public interface IPermissionProtectedRequest
{
    /// <summary>
    /// Gets the collection of permissions required to execute this request.
    /// </summary>
    IEnumerable<string> RequiredPermissions { get; }
}
