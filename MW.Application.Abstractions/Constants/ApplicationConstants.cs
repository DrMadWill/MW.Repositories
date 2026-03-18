namespace MW.Application.Abstractions.Constants;

/// <summary>
/// Shared constants used across the application layer.
/// Centralizes magic values to avoid duplication and improve maintainability.
/// </summary>
public static class ApplicationConstants
{
    /// <summary>
    /// Default pagination settings.
    /// </summary>
    public static class Pagination
    {
        /// <summary>
        /// Default page number when not specified.
        /// </summary>
        public const int DefaultPage = 1;

        /// <summary>
        /// Default number of items per page.
        /// </summary>
        public const int DefaultPageSize = 20;

        /// <summary>
        /// Maximum allowed page size.
        /// </summary>
        public const int MaxPageSize = 100;
    }

    /// <summary>
    /// Default cache settings.
    /// </summary>
    public static class Cache
    {
        /// <summary>
        /// Default cache expiration in minutes.
        /// </summary>
        public const int DefaultExpirationMinutes = 5;

        /// <summary>
        /// Extended cache expiration in minutes for rarely changing data.
        /// </summary>
        public const int ExtendedExpirationMinutes = 30;
    }

    /// <summary>
    /// Standard error codes used across the application.
    /// </summary>
    public static class ErrorCodes
    {
        /// <summary>Error code for validation failures.</summary>
        public const string Validation = "Validation";

        /// <summary>Error code for not found resources.</summary>
        public const string NotFound = "NotFound";

        /// <summary>Error code for conflict errors.</summary>
        public const string Conflict = "Conflict";

        /// <summary>Error code for unauthorized access.</summary>
        public const string Unauthorized = "Unauthorized";

        /// <summary>Error code for forbidden access.</summary>
        public const string Forbidden = "Forbidden";

        /// <summary>Error code for business rule violations.</summary>
        public const string BusinessRule = "BusinessRule";
    }
}
