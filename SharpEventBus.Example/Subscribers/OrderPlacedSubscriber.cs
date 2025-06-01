using SharpEventBus.Example.Events;
using SharpEventBus.Subscriber;

namespace SharpEventBus.Example.Subscribers;

// Subscriber that handles OrderPlacedEvent events
public sealed class OrderPlacedSubscriber : SubscriberBase<OrderPlacedEvent>
{
    // Called when an OrderPlacedEvent is published
    public override void OnEvent(OrderPlacedEvent e)
    {
        Console.WriteLine($"Order placed: {e.OrderId} at {e.Timestamp}");
    }
}