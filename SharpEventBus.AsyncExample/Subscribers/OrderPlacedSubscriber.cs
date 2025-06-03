using SharpEventBus.AsyncExample.Events;
using SharpEventBus.Subscriber;

namespace SharpEventBus.AsyncExample.Subscribers;

public sealed class OrderPlacedAsyncSubscriber : AsyncEventSubscriberBase<OrderPlacedEvent>
{
    public override async Task OnEventAsync(OrderPlacedEvent e)
    {
        await Task.Delay(Random.Shared.Next(0, 500));

        Console.WriteLine($"Order placed: {e.OrderId} at {e.Timestamp}");
    }
}
