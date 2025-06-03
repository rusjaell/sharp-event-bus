using SharpEventBus.AsyncExample.Events;
using SharpEventBus.Subscriber;

namespace SharpEventBus.AsyncExample.Subscribers;

public sealed class OrderCancelledAsyncSubscriber : AsyncEventSubscriberBase<OrderCancelledEvent>
{
    public override Task OnEventAsync(OrderCancelledEvent e)
    {
        //Console.WriteLine($"Order cancelled: {e.OrderId} reason: {e.Reason}");
        return Task.CompletedTask;
    }
}