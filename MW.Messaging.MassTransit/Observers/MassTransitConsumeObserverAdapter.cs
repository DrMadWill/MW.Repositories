using MassTransit;
using MW.Messaging.Constants;
using MessageHeaders = MW.Messaging.Headers.MessageHeaders;
using MW.Messaging.Observability;

namespace MW.Messaging.MassTransit.Observers;

public class MassTransitConsumeObserverAdapter : global::MassTransit.IConsumeObserver
{
    private readonly MW.Messaging.MassTransit.IConsumeObserver? _observer;

    public MassTransitConsumeObserverAdapter(MW.Messaging.MassTransit.IConsumeObserver? observer = null)
    {
        _observer = observer;
    }

    public async Task PreConsume<T>(ConsumeContext<T> context) where T : class
    {
        if (_observer == null) return;
        var logContext = BuildLogContext(context, EventStatuses.Pending);
        await _observer.OnPreConsume(logContext);
    }

    public async Task PostConsume<T>(ConsumeContext<T> context) where T : class
    {
        if (_observer == null) return;
        var logContext = BuildLogContext(context, EventStatuses.Success);
        await _observer.OnPostConsume(logContext);
    }

    public async Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception) where T : class
    {
        if (_observer == null) return;
        var logContext = BuildLogContext(context, EventStatuses.Failed);
        await _observer.OnConsumeFault(logContext, exception);
    }

    private static MessageLogContext BuildLogContext<T>(ConsumeContext<T> context, string status) where T : class
    {
        return new MessageLogContext
        {
            MessageId = context.MessageId,
            EventName = GetHeaderValue(context, MessageHeaders.EventName) ?? typeof(T).Name,
            EventVersion = GetHeaderValue(context, MessageHeaders.EventVersion),
            CorrelationId = GetHeaderValue(context, MessageHeaders.CorrelationId),
            CausationId = GetHeaderValue(context, MessageHeaders.CausationId),
            TraceId = GetHeaderValue(context, MessageHeaders.TraceId),
            SourceService = GetHeaderValue(context, MessageHeaders.SourceService),
            Consumer = context.ReceiveContext.InputAddress?.Segments.LastOrDefault()?.Trim('/'),
            Endpoint = context.ReceiveContext.InputAddress?.ToString(),
            Status = status,
            Timestamp = DateTimeOffset.UtcNow
        };
    }

    private static string? GetHeaderValue<T>(ConsumeContext<T> context, string key) where T : class
    {
        return context.Headers.TryGetHeader(key, out var value) ? value?.ToString() : null;
    }
}
