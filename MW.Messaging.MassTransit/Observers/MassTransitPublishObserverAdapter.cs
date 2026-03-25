using MassTransit;
using MW.Messaging.Constants;
using MessageHeaders = MW.Messaging.Headers.MessageHeaders;
using MW.Messaging.Observability;

namespace MW.Messaging.MassTransit.Observers;

public class MassTransitPublishObserverAdapter : global::MassTransit.IPublishObserver
{
    private readonly MW.Messaging.MassTransit.IPublishObserver? _observer;

    public MassTransitPublishObserverAdapter(MW.Messaging.MassTransit.IPublishObserver? observer = null)
    {
        _observer = observer;
    }

    public async Task PrePublish<T>(PublishContext<T> context) where T : class
    {
        if (_observer == null) return;
        var logContext = BuildLogContext(context, EventStatuses.Pending);
        await _observer.OnPrePublish(logContext);
    }

    public async Task PostPublish<T>(PublishContext<T> context) where T : class
    {
        if (_observer == null) return;
        var logContext = BuildLogContext(context, EventStatuses.Success);
        await _observer.OnPostPublish(logContext);
    }

    public async Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class
    {
        if (_observer == null) return;
        var logContext = BuildLogContext(context, EventStatuses.Failed);
        await _observer.OnPublishFault(logContext, exception);
    }

    private static MessageLogContext BuildLogContext<T>(PublishContext<T> context, string status) where T : class
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

    private static string? GetHeaderValue<T>(PublishContext<T> context, string key) where T : class
    {
        return context.Headers.TryGetHeader(key, out var value) ? value?.ToString() : null;
    }
}
