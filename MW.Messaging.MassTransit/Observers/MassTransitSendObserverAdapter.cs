using MassTransit;
using MW.Messaging.Constants;
using MessageHeaders = MW.Messaging.Headers.MessageHeaders;
using MW.Messaging.Observability;

namespace MW.Messaging.MassTransit.Observers;

public class MassTransitSendObserverAdapter : global::MassTransit.ISendObserver
{
    private readonly MW.Messaging.MassTransit.ISendObserver? _observer;

    public MassTransitSendObserverAdapter(MW.Messaging.MassTransit.ISendObserver? observer = null)
    {
        _observer = observer;
    }

    public async Task PreSend<T>(SendContext<T> context) where T : class
    {
        if (_observer == null) return;
        var logContext = BuildLogContext(context, EventStatuses.Pending);
        await _observer.OnPreSend(logContext);
    }

    public async Task PostSend<T>(SendContext<T> context) where T : class
    {
        if (_observer == null) return;
        var logContext = BuildLogContext(context, EventStatuses.Success);
        await _observer.OnPostSend(logContext);
    }

    public async Task SendFault<T>(SendContext<T> context, Exception exception) where T : class
    {
        if (_observer == null) return;
        var logContext = BuildLogContext(context, EventStatuses.Failed);
        await _observer.OnSendFault(logContext, exception);
    }

    private static MessageLogContext BuildLogContext<T>(SendContext<T> context, string status) where T : class
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
            Endpoint = context.DestinationAddress?.ToString(),
            Status = status,
            Timestamp = DateTimeOffset.UtcNow
        };
    }

    private static string? GetHeaderValue<T>(SendContext<T> context, string key) where T : class
    {
        return context.Headers.TryGetHeader(key, out var value) ? value?.ToString() : null;
    }
}
