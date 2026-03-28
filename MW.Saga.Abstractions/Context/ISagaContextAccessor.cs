namespace MW.Saga.Context;

/// <summary>
/// Scoped accessor for the currently active saga context.
/// Provides a safe way for application and infrastructure code to access
/// saga execution metadata within the current scope.
/// <para>
/// Implementations should be scoped per saga execution. The <see cref="Current"/> property
/// is expected to be set by the transport integration at the start of each saga activity
/// and is safe for scoped runtime usage within that activity.
/// </para>
/// <para>
/// This abstraction remains transport-agnostic. Concrete implementations
/// belong in infrastructure packages such as <c>MW.Saga.MassTransit</c>.
/// </para>
/// </summary>
public interface ISagaContextAccessor
{
    /// <summary>
    /// Gets the current saga context, or <c>null</c> if no saga execution is active.
    /// </summary>
    ISagaContext? Current { get; }
}
