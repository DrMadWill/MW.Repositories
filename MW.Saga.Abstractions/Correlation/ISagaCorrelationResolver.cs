namespace MW.Saga.Correlation;

/// <summary>
/// Contract for resolving saga correlation information from incoming messages or events.
/// <para>
/// Implementations are responsible for extracting correlation data (such as
/// <see cref="Guid"/> correlation identifiers) from a given message so that
/// the saga infrastructure can route the message to the correct saga instance.
/// </para>
/// <para>
/// The abstraction is generic and does not assume a specific transport,
/// serialization model, or message envelope structure.
/// </para>
/// </summary>
/// <typeparam name="TMessage">The type of the incoming message or event.</typeparam>
public interface ISagaCorrelationResolver<in TMessage> where TMessage : class
{
    /// <summary>
    /// Resolves the saga correlation identifier from the given message.
    /// </summary>
    /// <param name="message">The incoming message or event to resolve correlation from.</param>
    /// <returns>The resolved correlation identifier for the saga instance.</returns>
    Guid Resolve(TMessage message);
}
