namespace MW.Messaging.MassTransit;

/// <summary>
/// Abstraction for a reusable publish observer that performs structured logging
/// around MassTransit publish operations.
/// </summary>
public interface IPublishObserver
{
    /// <summary>
    /// Called before a message is published.
    /// </summary>
    /// <param name="logContext">The message log context with pre-publish metadata.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task OnPrePublish(Observability.MessageLogContext logContext);

    /// <summary>
    /// Called after a message is successfully published.
    /// </summary>
    /// <param name="logContext">The message log context with post-publish metadata.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task OnPostPublish(Observability.MessageLogContext logContext);

    /// <summary>
    /// Called when a publish operation fails.
    /// </summary>
    /// <param name="logContext">The message log context with failure metadata.</param>
    /// <param name="exception">The exception that caused the failure.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task OnPublishFault(Observability.MessageLogContext logContext, Exception exception);
}
