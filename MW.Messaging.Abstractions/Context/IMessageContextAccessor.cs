namespace MW.Messaging.Context;

/// <summary>
/// Provides read-only access to the current message context within a message-consuming flow.
/// <para>
/// Application and business code running inside a consumer can use this accessor to obtain
/// correlation, user, tenant, and message metadata that was resolved from the transport layer.
/// </para>
/// <para>
/// This abstraction does not expose raw MassTransit types. Transport-specific mapping
/// (e.g., from <c>ConsumeContext</c> headers) belongs to the implementation layer,
/// not to this contract.
/// </para>
/// </summary>
public interface IMessageContextAccessor
{
    /// <summary>
    /// Gets the current consumer context model, or <c>null</c> if no message-consuming flow is active.
    /// </summary>
    Messaging.ConsumerContextModel? Current { get; }
}
