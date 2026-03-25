using MW.Messaging.Context;
using MW.Messaging.Messaging;

namespace MW.Messaging.MassTransit.Context;

public class ScopedMessageContextAccessor : IMessageContextAccessor
{
    public ConsumerContextModel? Current { get; private set; }

    internal void SetContext(ConsumerContextModel context)
    {
        Current = context ?? throw new ArgumentNullException(nameof(context));
    }
}
