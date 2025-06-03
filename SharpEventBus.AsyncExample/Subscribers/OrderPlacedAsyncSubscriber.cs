using SharpEventBus.AsyncExample.Events;
using SharpEventBus.Subscriber;

namespace SharpEventBus.AsyncExample.Subscribers;

public sealed class OrderPlacedAsyncSubscriber : AsyncEventSubscriberBase<OrderPlacedEvent>
{
    public override async Task OnEventAsync(OrderPlacedEvent e)
    {
        // simulate IO operation
        await Task.Delay(Random.Shared.Next(0, 250));
    }
}
