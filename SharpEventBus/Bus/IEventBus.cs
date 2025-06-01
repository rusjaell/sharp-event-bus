using SharpEventBus.Event;
using SharpEventBus.Subscriber;

namespace SharpEventBus.Bus;

/// <summary>
/// Defines a framework for an event bus that supports publishing events, subscribing to events, and consumption of queued events.
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publishes an event to the bus.
    /// </summary>
    /// <typeparam name="T">The type of event being published.</typeparam>
    /// <param name="e">The event instance to publish.</param>
    void PublishEvent<T>(T e) where T : class, IEvent;

    /// <summary>
    /// Subscribes a handler to receive notifications for a specific event type.
    /// </summary>
    /// <typeparam name="T">The type of event to subscribe to.</typeparam>
    /// <param name="subscriber">The subscriber instance that will handle the event.</param>
    void Subscribe<T>(ISubscriber<T> subscriber) where T : class, IEvent;

    /// <summary>
    /// Processes all queued events.
    /// </summary>
    void ConsumeEvents();

    /// <summary>
    /// Attempts to process a single event from the queue test.
    /// </summary>
    /// <returns>
    /// <c>true</c> if an event was dequeued and processed; otherwise, <c>false</c> if no events were available.
    /// </returns>
    bool ConsumeOneEvent();
}
