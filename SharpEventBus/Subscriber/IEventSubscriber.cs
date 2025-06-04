using SharpEventBus.Event;

namespace SharpEventBus.Subscriber;

/// <summary>
/// Defines a subscriber that handles events.
/// </summary>
public interface IEventSubscriber
{
    /// <summary>
    /// Handles the received event.
    /// </summary>
    /// <param name="e">The event to handle.</param>
    void OnEvent(IEvent e);
}

/// <summary>
/// Defines a subscriber that handles events of a specific type.
/// </summary>
/// <typeparam name="TEvent">The type of event this subscriber handles.</typeparam>
public interface IEventSubscriber<TEvent> : IEventSubscriber where TEvent : IEvent
{
    /// <summary>
    /// Handles the received event of type <typeparamref name="TEvent"/>.
    /// </summary>
    /// <param name="e">The event to handle.</param>
    void OnEvent(TEvent e);
}
