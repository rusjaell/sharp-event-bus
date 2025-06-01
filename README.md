# SharpEventBus
A simple, lightweight, in-memory event bus library for modern .NET applications.

## Overview
SharpEventBus provides a synchronous event bus that allows publishing and subscribing to events within your application. It supports event queueing and dispatching synchronously.

## Planned Features
- [x] Configurable Settings  
- [ ] Event Triggers  
- [ ] Event Hooks Support  
- [ ] Event Chaining Support  
- [ ] Event Filtering  
- [ ] Event Priorities  
- [ ] Event Scheduler  
- [ ] Asynchronous Publishing and Consumption Support  

## Usage Example

```csharp
using SharpEventBus;
using SharpEventBus.Events;
using SharpEventBus.Subscribers;

// Create a default EventBus instance
var eventBus = EventBusBuilder.Create();

// Create a custom EventBus with options
var customEventBus = EventBusBuilder.Create(options =>
{
    // Use a custom event queue implementation
    options.WithEventQueue(new CustomEventQueue());

    // Or use a custom event dispatcher
    options.WithEventDispatcher(new CustomEventDispatcher());
});

// Subscribe to events
eventBus.Subscribe(new OrderPlacedSubscriber());

// Publish an event to subscribers
eventBus.PublishEvent(new OrderPlacedEvent("Order123", DateTime.UtcNow));

// Consume all pending events synchronously
eventBus.ConsumeEvents();

// Consume a single event manually
eventBus.ConsumeOneEvent();

// Event model definition
public record OrderPlacedEvent(string OrderId, DateTime Timestamp) : IEvent;

// Subscriber implementation
public class OrderPlacedSubscriber : SubscriberBase<OrderPlacedEvent>
{
    public override void OnEvent(OrderPlacedEvent e)
    {
        Console.WriteLine($"Order placed: {e.OrderId} at {e.Timestamp}");
    }
}
