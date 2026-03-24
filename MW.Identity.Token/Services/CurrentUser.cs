using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MW.Identity.Token.Constants;
using MW.Identity.Token.Contracts;
using Newtonsoft.Json;

namespace MW.Identity.Token.Services;

/// <summary>
/// Provides access to the current user's identity information from the HTTP context.
/// Implements <see cref="ICurrentUser"/> and provides additional helper methods
/// for claims extraction, role checks, and header access.
/// </summary>
public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentUser"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

    /// <summary>
    /// Gets the claims principal from the current HTTP context, or null if not authenticated.
    /// </summary>
    public ClaimsPrincipal? ClaimsPrincipal =>
        IsAuthenticated ? _httpContextAccessor.HttpContext?.User : null;

    /// <inheritdoc />
    public string? UserId =>
        IsAuthenticated ? ClaimsPrincipal?.FindFirst(ClaimConstants.UserId)?.Value : null;

    /// <inheritdoc />
    public string? Email =>
        IsAuthenticated ? ClaimsPrincipal?.FindFirst(ClaimConstants.Email)?.Value : null;

    /// <summary>
    /// Gets a value indicating whether the current user is a super admin.
    /// </summary>
    public bool IsSuperAdmin =>
        IsAuthenticated && ClaimsPrincipal?.IsInRole(SystemRole.SuperAdmin) == true;

    /// <inheritdoc />
    public bool IsInRole(string role) =>
        IsAuthenticated && ClaimsPrincipal?.IsInRole(role) == true;

    /// <inheritdoc />
    public IList<string> Roles
    {
        get
        {
            if (!IsAuthenticated || ClaimsPrincipal == null)
                return new List<string>();

            return ClaimsPrincipal.Claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type == ClaimConstants.Role)
                .Select(c => c.Value)
                .Distinct()
                .ToList();
        }
    }

    /// <summary>
    /// Gets a claim value by type.
    /// </summary>
    /// <param name="type">The claim type.</param>
    /// <returns>The claim value, or null if not found.</returns>
    public string? Get(string type) =>
        HasClaimType(type) ? ClaimsPrincipal?.FindFirst(type)?.Value : null;

    /// <summary>
    /// Gets a claim value deserialized from JSON.
    /// </summary>
    /// <typeparam name="T">The target type for deserialization.</typeparam>
    /// <param name="type">The claim type.</param>
    /// <returns>The deserialized value, or default if not found.</returns>
    public T? GetJson<T>(string type)
    {
        var value = Get(type);
        if (!string.IsNullOrEmpty(value))
            return JsonConvert.DeserializeObject<T>(value);
        return default;
    }

    /// <summary>
    /// Checks whether the specified claim type exists for the current user.
    /// </summary>
    /// <param name="type">The claim type to check.</param>
    /// <returns>true if the claim type exists; otherwise, false.</returns>
    public bool HasClaimType(string type) =>
        IsAuthenticated && ClaimsPrincipal?.HasClaim(s => s.Type == type) == true;

    /// <summary>
    /// Checks whether data access should be granted to the user.
    /// Returns true if the user is a super admin or if the userId matches the current user.
    /// </summary>
    /// <param name="userId">The user ID to check authorization for.</param>
    /// <returns>true if the user is authorized; otherwise, false.</returns>
    public bool CheckUserAuthorize(string userId)
    {
        if (ClaimsPrincipal == null || !IsAuthenticated) return false;
        if (IsSuperAdmin) return true;
        return UserId == userId;
    }

    /// <summary>
    /// Gets an HTTP header value by key.
    /// </summary>
    /// <param name="key">The header key.</param>
    /// <returns>The header value, or null if not found or empty.</returns>
    public string? GetHeaderValue(string key)
    {
        var value = _httpContextAccessor.HttpContext?.Request?.Headers[key].ToString();
        return string.IsNullOrEmpty(value) ? null : value;
    }
}
