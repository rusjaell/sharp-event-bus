using SharpEventBus.Event;
using SharpEventBus.Subscriber;

namespace SharpEventBus.Bus;

/// <summary>
/// Defines a framework for an async event bus that supports publishing events, subscribing to events.
/// </summary>
public interface IAsyncEventBus
{
    /// <summary>
    /// Publishes an event to the bus.
    /// </summary>
    /// <typeparam name="T">The type of event being published.</typeparam>
    /// <param name="e">The event instance to publish.</param>
    void Publish<T>(T e) where T : class, IEvent;

    /// <summary>
    /// Subscribes a handler to receive notifications for a specific event type.
    /// </summary>
    /// <typeparam name="T">The type of event to subscribe to.</typeparam>
    /// <param name="subscriber">The subscriber instance that will handle the event.</param>
    void AddSubscriber<T>(IAsyncEventSubscriber<T> subscriber) where T : class, IEvent;
}