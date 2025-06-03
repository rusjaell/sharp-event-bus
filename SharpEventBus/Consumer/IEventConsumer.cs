using SharpEventBus.Event;
using SharpEventBus.Subscriber;

namespace SharpEventBus.Consumer;

/// <summary>
/// Represents a consumer that processes events by enqueueing them,
/// managing subscribers, and consuming events synchronously.
/// </summary>
public interface IEventConsumer
{
    /// <summary>
    /// Adds an event to the processing queue.
    /// </summary>
    /// <param name="e">The event to enqueue.</param>
    void Enqueue(IEvent e);

    /// <summary>
    /// Registers a subscriber to receive events.
    /// </summary>
    /// <param name="subscriber">The subscriber to add.</param>
    void AddSubscriber(IEventSubscriber subscriber);

    /// <summary>
    /// Starts consuming and processing queued events synchronously.
    /// </summary>
    void ConsumeEvents();
}