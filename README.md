# SharpEventBus
A simple, lightweight, in-memory event bus library for modern .NET applications.

## Overview
SharpEventBus provides thread-safe asynchronous and synchronous event bus implementations, allowing you to easily publish and subscribe to events within your domain application.

## Features
- ✔️ Configurable Settings
- ✔️ Event Publishing
- ✔️ Event Subscribing
- ✔️ Event Consumption (Asynchronous & Syncronous)

### Notes
- 🔄 Async mode automatically consumes events in the background.
- ⚙️ Sync mode requires manual consumption via ConsumeEvents();

## Syncronous Example

```csharp
using SharpEventBus;
using SharpEventBus.Events;
using SharpEventBus.Subscribers;

// Manual Custom Configuration
var configuration = EventBusConfigurationBuilder.Create(builder =>
{
    builder.WithDebugLogging();
});

// Custom configuration
var eventBus = SyncEventBusBuilder.Create(options =>
{
    // With a manual builder
    options.WithConfiguration(configuration);

    // With a auto builder
    options.WithConfiguration(builder =>
    {
        builder.WithDebugLogging();
    });
});

// Create a default EventBus instance with all internal defaults
var eventBus = SyncEventBusBuilder.Create();

// Create a custom EventBus with options
var eventBus = SyncEventBusBuilder.Create(options =>
{
    // Add custom implementations, or dont and use for default
    options.WithEventQueueFactory(() => new CustomEventQueue());
    options.WithEventDispatcherFactory(() => new CustomEventDispatcher());
});

// Subscribe to events

// Manually Create and add a subscriber
var orderPlacedSubscriber = eventBus.AddSubscriber(new OrderPlacedAsyncSubscriber());
var orderCancelledSubscriber = eventBus.AddSubscriber(new OrderCancelledAsyncSubscriber());

// Automatically Create and add a subscriber
var orderPlacedSubscriber = eventBus.RegisterSubscriber<OrderPlacedAsyncSubscriber, OrderPlacedEvent>();
var orderCancelledSubscriber = eventBus.RegisterSubscriber<OrderCancelledAsyncSubscriber, OrderCancelledEvent>();

// Publish an event to subscribers
eventBus.Publish(new OrderPlacedEvent("Order123", DateTime.UtcNow));

// Manually have to call ConsumeEvents to consume events
eventBus.ConsumeEvents();

// Event model definition
public record OrderPlacedEvent(string OrderId, DateTime Timestamp) : IEvent;

// Subscriber implementation
public sealed class OrderPlacedSubscriber : SubscriberBase<OrderPlacedEvent>
{
    public override void OnEvent(OrderPlacedEvent e)
    {
        Console.WriteLine($"Order placed: {e.OrderId} at {e.Timestamp}");
    }
}
```

## Asynchronous Example

```csharp
using SharpEventBus;
using SharpEventBus.Events;
using SharpEventBus.Subscribers;

// Manual Custom Configuration
var configuration = EventBusConfigurationBuilder.Create(builder =>
{
    builder.WithDebugLogging();
    builder.WithMaxConsumerConcurrency(16);
});

// Custom configuration
var asyncEventBus = AsyncEventBusBuilder.Create(builder =>
{
    // With a manual builder
    options.WithConfiguration(configuration);

    // With a auto builder
    options.WithConfiguration(builder =>
    {
        builder.WithDebugLogging();
        builder.WithMaxConsumerConcurrency(16);
    });
});

// Create a default EventBus instance with all internal defaults
var eventBus = SyncEventBusBuilder.Create();

// Create a custom EventBus with options
var eventBus = SyncEventBusBuilder.Create(options =>
{
    // Add custom implementations, or dont and use for default
    options.WithEventQueueFactory(() => new CustomEventQueue());
    options.WithEventDispatcherFactory(() => new CustomEventDispatcher());
});

// Subscribe to events
// Note - Will automatically start to Consume events related to subscriber once the subscriber is added

// Manually Create and add a subscriber
//var orderPlacedSubscriber = asyncEventBus.AddSubscriber(new OrderPlacedSubscriber());
//var orderCancelledSubscriber = asyncEventBus.AddSubscriber(new OrderCancelledSubscriber());

// Automatically Create and add a subscriber
var orderPlacedSubscriber = eventBus.RegisterSubscriber<OrderPlacedSubscriber, OrderPlacedEvent>();
var orderCancelledSubscriber = eventBus.RegisterSubscriber<OrderCancelledSubscriber, OrderCancelledEvent>();

// Publish an event to subscribers
asyncEventBus.Publish(new OrderPlacedEvent("Order123", DateTime.UtcNow));

// Event model definition
public record OrderPlacedEvent(string OrderId, DateTime Timestamp) : IEvent;

// Subscriber implementation
public sealed class OrderPlacedAsyncSubscriber : AsyncEventSubscriberBase<OrderPlacedEvent>
{
    public override async Task OnEventAsync(OrderPlacedEvent e)
    {
        // Simulated IO Work
        await Task.Delay(Random.Shared.Next(0, 50));

        Console.WriteLine($"Order placed: {e.OrderId} at {e.Timestamp}");
    }
}
```
