using MW.Saga.Context;
using MW.Saga.Models;

namespace MW.Saga.MassTransit.Context;

/// <summary>
/// Mutable implementation of <see cref="ISagaContext"/> used during saga execution.
/// Populated by the saga context filter and exposed as read-only through the accessor.
/// </summary>
internal class MutableSagaContext : ISagaContext
{
    /// <inheritdoc />
    public Guid CorrelationId { get; set; }

    /// <inheritdoc />
    public string CurrentState { get; set; } = string.Empty;

    /// <inheritdoc />
    public SagaStatus Status { get; set; }

    /// <inheritdoc />
    public string? SourceService { get; set; }

    /// <inheritdoc />
    public string? StartedByEvent { get; set; }

    /// <inheritdoc />
    public string? TraceId { get; set; }
}
