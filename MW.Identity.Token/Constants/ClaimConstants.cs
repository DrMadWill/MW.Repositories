namespace MW.Identity.Token.Constants;

/// <summary>
/// Constants for claim type names used in JWT tokens.
/// </summary>
public static class ClaimConstants
{
    /// <summary>
    /// The claim type for the user's unique identifier.
    /// </summary>
    public const string UserId = "userId";

    /// <summary>
    /// The claim type for the user's username.
    /// </summary>
    public const string UserName = "userName";

    /// <summary>
    /// The claim type for the user's display name.
    /// </summary>
    public const string DisplayName = "displayName";

    /// <summary>
    /// The claim type for the user's phone number.
    /// </summary>
    public const string Phone = "phone";

    /// <summary>
    /// The claim type for the user's name.
    /// </summary>
    public const string Name = "name";

    /// <summary>
    /// The claim type for the user's email address.
    /// </summary>
    public const string Email = "email";

    /// <summary>
    /// The claim type for the token expiration.
    /// </summary>
    public const string Expiration = "expiration";

    /// <summary>
    /// The claim type for the system identifier.
    /// </summary>
    public const string SystemId = "systemId";

    /// <summary>
    /// The claim type for a single role.
    /// </summary>
    public const string Role = "role";

    /// <summary>
    /// The claim type for roles.
    /// </summary>
    public const string Roles = "roles";

    /// <summary>
    /// The claim type for the user's Telegram chat identifier.
    /// </summary>
    public const string TelegramChatId = "telegramChatId";

    /// <summary>
    /// The claim type for the user's account creation date.
    /// </summary>
    public const string CreatedDate = "createdDate";

    /// <summary>
    /// The claim type for the shop point identifier.
    /// </summary>
    public const string ShopPointId = "shopPointId";

    /// <summary>
    /// The claim type for the shop point name.
    /// </summary>
    public const string ShopPointName = "shopPointName";

    /// <summary>
    /// The claim type indicating whether the user has a temporary password.
    /// </summary>
    public const string IsTemporaryPassword = "isTemporaryPassword";

    /// <summary>
    /// The claim type for the marketplace user identifier who created the account.
    /// </summary>
    public const string CreatedMarketPlaceUserId = "createdMarketPlaceUserId";
}
