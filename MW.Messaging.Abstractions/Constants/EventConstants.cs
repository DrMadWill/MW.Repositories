namespace MW.Messaging.Constants;

/// <summary>
/// Shared constants for event directions used in audit records and logging.
/// </summary>
public static class EventDirections
{
    /// <summary>
    /// Indicates the event was published by the service.
    /// </summary>
    public const string Published = "Published";

    /// <summary>
    /// Indicates the event was consumed by the service.
    /// </summary>
    public const string Consumed = "Consumed";
}

/// <summary>
/// Shared constants for event processing statuses used in audit records and logging.
/// </summary>
public static class EventStatuses
{
    /// <summary>
    /// Indicates the event processing was successful.
    /// </summary>
    public const string Success = "Success";

    /// <summary>
    /// Indicates the event processing failed.
    /// </summary>
    public const string Failed = "Failed";

    /// <summary>
    /// Indicates the event is pending processing.
    /// </summary>
    public const string Pending = "Pending";
}
