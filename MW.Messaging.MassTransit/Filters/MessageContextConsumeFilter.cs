using MassTransit;
using MW.Messaging.Headers;

namespace MW.Messaging.MassTransit.Filters;

public class MessageContextConsumeFilter<TMessage> : IFilter<ConsumeContext<TMessage>>
    where TMessage : class
{
    private readonly Context.ScopedMessageContextAccessor _accessor;
    private readonly MW.Messaging.MassTransit.IMessageHeaderMapper _headerMapper;

    public MessageContextConsumeFilter(
        Context.ScopedMessageContextAccessor accessor,
        MW.Messaging.MassTransit.IMessageHeaderMapper headerMapper)
    {
        _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        _headerMapper = headerMapper ?? throw new ArgumentNullException(nameof(headerMapper));
    }

    public async Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
    {
        var headers = new Dictionary<string, object>();

        foreach (var header in context.Headers.GetAll())
        {
            if (header.Value != null)
                headers[header.Key] = header.Value;
        }

        var consumerContext = _headerMapper.MapFromHeaders(headers);
        consumerContext.MessageId = context.MessageId ?? Guid.NewGuid();

        _accessor.SetContext(consumerContext);

        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("messageContextPropagation");
    }
}
