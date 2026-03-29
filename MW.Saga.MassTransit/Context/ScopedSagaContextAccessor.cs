using MW.Saga.Context;
using MW.Saga.Models;

namespace MW.Saga.MassTransit.Context;

/// <summary>
/// Scoped accessor for the currently active saga context.
/// Infrastructure code populates the context at the start of saga execution
/// and clears it when execution completes.
/// Application code reads the context through <see cref="ISagaContextAccessor"/>.
/// </summary>
internal class ScopedSagaContextAccessor : ISagaContextAccessor
{
    /// <inheritdoc />
    public ISagaContext? Current { get; private set; }

    /// <summary>
    /// Sets the current saga context. Called by infrastructure filters/middleware.
    /// </summary>
    internal void SetContext(ISagaContext context)
    {
        Current = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Clears the current saga context. Called after saga execution completes.
    /// </summary>
    internal void ClearContext()
    {
        Current = null;
    }
}
