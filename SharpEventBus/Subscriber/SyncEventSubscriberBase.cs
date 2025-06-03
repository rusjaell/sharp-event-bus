using SharpEventBus.Event;

namespace SharpEventBus.Subscriber;

/// <summary>
/// Base class for subscribers that handle events of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of event to handle.</typeparam>
public abstract class SyncEventSubscriberBase<T> : IEventSubscriber<T> where T : IEvent
{
    /// <summary>
    /// Handles the event of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="e">The event to handle.</param>
    public abstract void OnEvent(T e);

    /// <summary>
    /// Handles the event using the non-generic interface by casting the event to type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="e">The event to handle.</param>
    void IEventSubscriber.OnEvent(IEvent e) => OnEvent((T)e);
}