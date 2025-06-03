using SharpEventBus.Subscriber;
using SharpEventBus.SyncExample.Events;

namespace SharpEventBus.SyncExample.Subscribers;

// Subscriber that handles OrderPlacedEvent events
public sealed class OrderPlacedSubscriber : SyncEventSubscriberBase<OrderPlacedEvent>
{
    // Called when an OrderPlacedEvent is published
    public override void OnEvent(OrderPlacedEvent e)
    {
        Console.WriteLine($"Order placed: {e.OrderId} at {e.Timestamp}");
    }
}