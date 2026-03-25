namespace MW.Messaging.Validation;

/// <summary>
/// Defines the convention for formatting and normalizing integration event names and versions.
/// <para>
/// This abstraction separates naming convention logic (formatting, normalization)
/// from validation logic (<see cref="IIntegrationEventValidator"/>).
/// Implementations may enforce rules such as <c>{domain}.{action}.{version}</c> patterns,
/// kebab-case formatting, or version string normalization.
/// </para>
/// <para>
/// See <c>Docs/EventNamingConvention.md</c> for the current naming rules.
/// </para>
/// </summary>
public interface IEventNamingConvention
{
    /// <summary>
    /// Normalizes the given event name according to the convention.
    /// </summary>
    /// <param name="eventName">The raw event name.</param>
    /// <returns>The normalized event name.</returns>
    string NormalizeEventName(string eventName);

    /// <summary>
    /// Normalizes the given event version string according to the convention.
    /// </summary>
    /// <param name="eventVersion">The raw event version.</param>
    /// <returns>The normalized event version.</returns>
    string NormalizeEventVersion(string eventVersion);
}
