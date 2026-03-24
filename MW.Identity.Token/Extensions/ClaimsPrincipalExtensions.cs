using System.Security.Claims;
using MW.Identity.Token.Constants;

namespace MW.Identity.Token.Extensions;

/// <summary>
/// Extension methods for <see cref="ClaimsPrincipal"/> to simplify access to common claims.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets a claim value by type.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal.</param>
    /// <param name="type">The claim type.</param>
    /// <returns>The claim value, or null if not found.</returns>
    public static string? Get(this ClaimsPrincipal claimsPrincipal, string type)
        => claimsPrincipal.FindFirst(type)?.Value;

    /// <summary>
    /// Gets the user ID from claims.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal.</param>
    /// <returns>The user ID.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when user ID is not found in claims.</exception>
    public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var userId = claimsPrincipal.Get(ClaimConstants.UserId);
        if (userId == null) throw new UnauthorizedAccessException("User ID not found in claims");
        return userId;
    }

    /// <summary>
    /// Gets the username from claims.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal.</param>
    /// <returns>The username, or null if not found.</returns>
    public static string? GetUserName(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.Get(ClaimConstants.UserName);

    /// <summary>
    /// Gets the user's email from claims.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal.</param>
    /// <returns>The email, or null if not found.</returns>
    public static string? GetUserEmail(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.Get(ClaimConstants.Email);

    /// <summary>
    /// Gets the user's display name from claims.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal.</param>
    /// <returns>The display name, or null if not found.</returns>
    public static string? GetUserDisplayName(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.Get(ClaimConstants.DisplayName);

    /// <summary>
    /// Gets the user's phone number from claims.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal.</param>
    /// <returns>The phone number, or null if not found.</returns>
    public static string? GetUserPhone(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.Get(ClaimConstants.Phone);

    /// <summary>
    /// Gets the user's account creation date from claims.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal.</param>
    /// <returns>The creation date, or null if not found.</returns>
    public static string? GetUserCreatedDate(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.Get(ClaimConstants.CreatedDate);

    /// <summary>
    /// Gets the user's Telegram chat ID from claims.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal.</param>
    /// <returns>The Telegram chat ID, or null if not found.</returns>
    public static string? GetTelegramChatId(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.Get(ClaimConstants.TelegramChatId);

    /// <summary>
    /// Gets the system ID from claims.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal.</param>
    /// <returns>The system ID, or null if not found.</returns>
    public static string? GetSystemId(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.Get(ClaimConstants.SystemId);

    /// <summary>
    /// Gets the token expiration from claims.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal.</param>
    /// <returns>The expiration value, or null if not found.</returns>
    public static string? GetExpiration(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.Get(ClaimConstants.Expiration);

    /// <summary>
    /// Gets the shop point ID from claims.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal.</param>
    /// <returns>The shop point ID, or null if not found.</returns>
    public static string? GetShopPointId(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.Get(ClaimConstants.ShopPointId);

    /// <summary>
    /// Gets the shop point name from claims.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal.</param>
    /// <returns>The shop point name, or null if not found.</returns>
    public static string? GetShopPointName(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.Get(ClaimConstants.ShopPointName);

    /// <summary>
    /// Gets the temporary password flag from claims.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal.</param>
    /// <returns>The temporary password flag, or null if not found.</returns>
    public static string? GetIsTemporaryPassword(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.Get(ClaimConstants.IsTemporaryPassword);

    /// <summary>
    /// Gets the marketplace user ID of the creator from claims.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal.</param>
    /// <returns>The creator marketplace user ID, or null if not found.</returns>
    public static string? GetCreatedMarketPlaceUserId(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.Get(ClaimConstants.CreatedMarketPlaceUserId);

    /// <summary>
    /// Gets the user's name from claims.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal.</param>
    /// <returns>The name, or null if not found.</returns>
    public static string? GetName(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.Get(ClaimConstants.Name);

    /// <summary>
    /// Determines whether the user is a super admin.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal.</param>
    /// <returns>true if the user is a super admin; otherwise, false.</returns>
    public static bool IsSuperAdmin(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.IsInRole(SystemRole.SuperAdmin);

    /// <summary>
    /// Determines whether the user is a product admin.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal.</param>
    /// <returns>true if the user is a product admin; otherwise, false.</returns>
    public static bool IsProductAdmin(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.IsInRole(SystemRole.ProductAdmin);

    /// <summary>
    /// Determines whether the user is a marketplace admin.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal.</param>
    /// <returns>true if the user is a marketplace admin; otherwise, false.</returns>
    public static bool IsMarketPlaceAdmin(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.IsInRole(SystemRole.MarketPlaceAdmin);

    /// <summary>
    /// Determines whether the user is a public shop admin.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal.</param>
    /// <returns>true if the user is a public shop admin; otherwise, false.</returns>
    public static bool IsPublicShopAdmin(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.IsInRole(SystemRole.PublicShopAdmin);

    /// <summary>
    /// Determines whether the user is an external system.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal.</param>
    /// <returns>true if the user is an external system; otherwise, false.</returns>
    public static bool IsExternalSystem(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.IsInRole(SystemRole.ExternalSystem);

    /// <summary>
    /// Determines whether the user has the User role.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal.</param>
    /// <returns>true if the user has the User role; otherwise, false.</returns>
    public static bool IsUser(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.IsInRole(SystemRole.User);

    /// <summary>
    /// Determines whether the user is an admin panel user (SuperAdmin or ProductAdmin).
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal.</param>
    /// <returns>true if the user is a super admin or product admin; otherwise, false.</returns>
    public static bool IsAdminPanelUser(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.IsInRole(SystemRole.SuperAdmin)
           || claimsPrincipal.IsInRole(SystemRole.ProductAdmin);
}
