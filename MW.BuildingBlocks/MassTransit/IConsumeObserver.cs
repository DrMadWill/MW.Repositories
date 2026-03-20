namespace MW.BuildingBlocks.MassTransit;

/// <summary>
/// Abstraction for a reusable consume observer that performs structured logging
/// around MassTransit consume operations.
/// </summary>
public interface IConsumeObserver
{
    /// <summary>
    /// Called before a message is consumed.
    /// </summary>
    /// <param name="logContext">The message log context with pre-consume metadata.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task OnPreConsume(Observability.MessageLogContext logContext);

    /// <summary>
    /// Called after a message is successfully consumed.
    /// </summary>
    /// <param name="logContext">The message log context with post-consume metadata.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task OnPostConsume(Observability.MessageLogContext logContext);

    /// <summary>
    /// Called when a consume operation fails.
    /// </summary>
    /// <param name="logContext">The message log context with failure metadata.</param>
    /// <param name="exception">The exception that caused the failure.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task OnConsumeFault(Observability.MessageLogContext logContext, Exception exception);
}
