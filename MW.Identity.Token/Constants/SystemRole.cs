namespace MW.Identity.Token.Constants;

/// <summary>
/// Constants for system role names used in authorization.
/// </summary>
public static class SystemRole
{
    /// <summary>
    /// The super admin role with full system access.
    /// </summary>
    public const string SuperAdmin = "SuperAdmin";

    /// <summary>
    /// The standard user role.
    /// </summary>
    public const string User = "User";

    /// <summary>
    /// The external system role for service-to-service communication.
    /// </summary>
    public const string ExternalSystem = "ExternalSystem";

    /// <summary>
    /// The product admin role for product management.
    /// </summary>
    public const string ProductAdmin = "ProductAdmin";

    /// <summary>
    /// The marketplace admin role.
    /// </summary>
    public const string MarketPlaceAdmin = "MarketPlaceAdmin";

    /// <summary>
    /// The public shop admin role.
    /// </summary>
    public const string PublicShopAdmin = "PublicShopAdmin";
}