namespace MW.Messaging.Identity;

/// <summary>
/// Provides the current service identity metadata in a standardized way.
/// <para>
/// Use this abstraction instead of hardcoding <c>SourceService</c> strings in application logic.
/// The returned <see cref="Messaging.ServiceIdentity"/> contains the minimum required
/// service identification metadata used by <see cref="Messaging.EventMetadata"/>
/// and <see cref="Contracts.IntegrationEvent"/>.
/// </para>
/// <para>
/// Implementations are expected to be transport-agnostic and environment-neutral,
/// resolving identity from configuration rather than runtime infrastructure.
/// </para>
/// </summary>
public interface IServiceIdentityProvider
{
    /// <summary>
    /// Gets the identity of the current service.
    /// </summary>
    /// <returns>A <see cref="Messaging.ServiceIdentity"/> representing the running service.</returns>
    Messaging.ServiceIdentity GetCurrent();
}
