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
/// <typeparam name="T">The type of event this subscriber handles.</typeparam>
public interface IEventSubscriber<T> : IEventSubscriber where T : IEvent
{
    /// <summary>
    /// Handles the received event of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="e">The event to handle.</param>
    void OnEvent(T e);
}
