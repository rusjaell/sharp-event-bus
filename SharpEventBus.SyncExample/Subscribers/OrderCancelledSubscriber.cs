using SharpEventBus.Subscriber;
using SharpEventBus.SyncExample.Events;

namespace SharpEventBus.SyncExample.Subscribers;

// Subscriber that handles OrderCancelledEvent events
public sealed class OrderCancelledSubscriber : SyncEventSubscriberBase<OrderCancelledEvent>
{
    // Called when an OrderCancelledEvent is published
    public override void OnEvent(OrderCancelledEvent e)
    {
        Console.WriteLine($"Order cancelled: {e.OrderId} reason: {e.Reason}");
    }
}