using SharpEventBus.Event;

namespace SharpEventBus.Subscriber;

/// <summary>
/// Base class for subscribers that handle events of type <typeparamref name="TEvent"/>.
/// </summary>
/// <typeparam name="TEvent">The type of event to handle.</typeparam>
public abstract class SyncEventSubscriberBase<TEvent> : IEventSubscriber<TEvent> where TEvent : IEvent
{
    /// <summary>
    /// Handles the event of type <typeparamref name="TEvent"/>.
    /// </summary>
    /// <param name="e">The event to handle.</param>
    public abstract void OnEvent(TEvent e);

    /// <summary>
    /// Handles the event using the non-generic interface by casting the event to type <typeparamref name="TEvent"/>.
    /// </summary>
    /// <param name="e">The event to handle.</param>
    void IEventSubscriber.OnEvent(IEvent e) => OnEvent((TEvent)e);
}