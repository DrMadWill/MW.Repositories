using MW.Saga.Context;
using MW.Saga.Models;

namespace MW.Saga.MassTransit.Context;

/// <summary>
/// Concrete runtime implementation of <see cref="ISagaExecutionContext"/>.
/// Populated from current saga/message runtime data via the saga context accessor.
/// Exposes saga execution metadata without leaking MassTransit runtime types.
/// </summary>
internal class MassTransitSagaExecutionContext : ISagaExecutionContext
{
    private readonly ISagaContextAccessor _accessor;

    public MassTransitSagaExecutionContext(ISagaContextAccessor accessor)
    {
        _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
    }

    /// <inheritdoc />
    public Guid CorrelationId => _accessor.Current?.CorrelationId ?? Guid.Empty;

    /// <inheritdoc />
    public string? CausationId => null;

    /// <inheritdoc />
    public string? TraceId => _accessor.Current?.TraceId;

    /// <inheritdoc />
    public string SagaName { get; internal set; } = string.Empty;

    /// <inheritdoc />
    public string CurrentState => _accessor.Current?.CurrentState ?? string.Empty;

    /// <inheritdoc />
    public SagaStatus Status => _accessor.Current?.Status ?? SagaStatus.NotStarted;

    /// <inheritdoc />
    public string? SourceService => _accessor.Current?.SourceService;
}
