using SharpEventBus.Event;

namespace SharpEventBus.Subscriber;

/// <summary>
/// Abstract base class for implementing strongly-typed asynchronous event subscribers.
/// Automatically handles casting from the non-generic interface.
/// </summary>
/// <typeparam name="TEvent">The type of event to handle.</typeparam>
public abstract class AsyncEventSubscriberBase<TEvent> : IAsyncEventSubscriber<TEvent> where TEvent : IEvent
{
    /// <inheritdoc/>
    public abstract Task OnEventAsync(TEvent e);

    /// <inheritdoc/>
    Task IAsyncEventSubscriber.OnEventAsync(IEvent e) => OnEventAsync((TEvent)e);
}