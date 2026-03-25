using MW.Messaging.Headers;
using MW.Messaging.Messaging;

namespace MW.Messaging.MassTransit.Context;

public class DefaultMessageHeaderMapper : MW.Messaging.MassTransit.IMessageHeaderMapper
{
    public IDictionary<string, object> MapToHeaders(PublishContextModel context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var headers = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(context.CorrelationId))
            headers[MessageHeaders.CorrelationId] = context.CorrelationId;

        if (!string.IsNullOrWhiteSpace(context.CausationId))
            headers[MessageHeaders.CausationId] = context.CausationId;

        if (context.TenantId.HasValue)
            headers[MessageHeaders.TenantId] = context.TenantId.Value.ToString();

        if (context.UserId.HasValue)
            headers[MessageHeaders.UserId] = context.UserId.Value.ToString();

        if (!string.IsNullOrWhiteSpace(context.SourceService))
            headers[MessageHeaders.SourceService] = context.SourceService;

        if (!string.IsNullOrWhiteSpace(context.TraceId))
            headers[MessageHeaders.TraceId] = context.TraceId;

        return headers;
    }

    public ConsumerContextModel MapFromHeaders(IDictionary<string, object> headers)
    {
        ArgumentNullException.ThrowIfNull(headers);

        var context = new ConsumerContextModel();

        if (headers.TryGetValue(MessageHeaders.CorrelationId, out var correlationId))
            context.CorrelationId = correlationId?.ToString();

        if (headers.TryGetValue(MessageHeaders.CausationId, out var causationId))
            context.CausationId = causationId?.ToString();

        if (headers.TryGetValue(MessageHeaders.TenantId, out var tenantId) &&
            Guid.TryParse(tenantId?.ToString(), out var parsedTenantId))
            context.TenantId = parsedTenantId;

        if (headers.TryGetValue(MessageHeaders.UserId, out var userId) &&
            Guid.TryParse(userId?.ToString(), out var parsedUserId))
            context.UserId = parsedUserId;

        if (headers.TryGetValue(MessageHeaders.SourceService, out var sourceService))
            context.SourceService = sourceService?.ToString();

        if (headers.TryGetValue(MessageHeaders.TraceId, out var traceId))
            context.TraceId = traceId?.ToString();

        if (headers.TryGetValue(MessageHeaders.EventName, out var eventName))
            context.EventName = eventName?.ToString();

        return context;
    }
}
