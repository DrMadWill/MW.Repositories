using System.Security.Claims;

namespace MW.Identity.Token.Contracts;

/// <summary>
/// Represents the current authenticated user context.
/// Provides access to user identity information, roles, and authentication state.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Gets a value indicating whether the current user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the claims principal from the current HTTP context, or null if not authenticated.
    /// Allows direct access to claims for advanced scenarios.
    /// </summary>
    ClaimsPrincipal? ClaimsPrincipal { get; }

    /// <summary>
    /// Gets the current user's unique identifier, or null if not authenticated.
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Gets the current user's email address, or null if not available.
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Determines whether the current user belongs to the specified role.
    /// </summary>
    /// <param name="role">The role name to check.</param>
    /// <returns>true if the user is in the specified role; otherwise, false.</returns>
    bool IsInRole(string role);

    /// <summary>
    /// Gets the list of roles assigned to the current user.
    /// </summary>
    IList<string> Roles { get; }
}
