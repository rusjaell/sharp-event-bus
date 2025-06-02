using SharpEventBus.Example.Events;
using SharpEventBus.Subscriber;

namespace SharpEventBus.Example.Subscribers;

// Subscriber that handles OrderCancelledEvent events
public sealed class OrderCancelledSubscriber : SyncEventSubscriberBase<OrderCancelledEvent>
{
    // Called when an OrderCancelledEvent is published
    public override void OnEvent(OrderCancelledEvent e)
    {
        Console.WriteLine($"Order cancelled: {e.OrderId} reason: {e.Reason}");
    }
}
