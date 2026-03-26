using MW.Messaging.Context;

namespace MW.Messaging.MassTransit.Context;

public class MassTransitMessageExecutionContext : IMessageExecutionContext
{
    private readonly IMessageContextAccessor _accessor;

    public MassTransitMessageExecutionContext(IMessageContextAccessor accessor)
    {
        _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
    }

    public string? CorrelationId => _accessor.Current?.CorrelationId;
    public string? CausationId => _accessor.Current?.CausationId;
    public string? TraceId => _accessor.Current?.TraceId;
    public Guid? UserId => _accessor.Current?.UserId;
    public Guid? TenantId => _accessor.Current?.TenantId;
    public string? SourceService => _accessor.Current?.SourceService;
    public Guid MessageId => _accessor.Current?.MessageId ?? Guid.Empty;
}
