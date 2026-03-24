namespace MW.Messaging.MassTransit;

/// <summary>
/// Abstraction for a reusable send observer that performs structured logging
/// around MassTransit send operations.
/// </summary>
public interface ISendObserver
{
    /// <summary>
    /// Called before a message is sent.
    /// </summary>
    /// <param name="logContext">The message log context with pre-send metadata.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task OnPreSend(Observability.MessageLogContext logContext);

    /// <summary>
    /// Called after a message is successfully sent.
    /// </summary>
    /// <param name="logContext">The message log context with post-send metadata.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task OnPostSend(Observability.MessageLogContext logContext);

    /// <summary>
    /// Called when a send operation fails.
    /// </summary>
    /// <param name="logContext">The message log context with failure metadata.</param>
    /// <param name="exception">The exception that caused the failure.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task OnSendFault(Observability.MessageLogContext logContext, Exception exception);
}
