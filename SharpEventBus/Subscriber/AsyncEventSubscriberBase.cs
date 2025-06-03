using SharpEventBus.Event;

namespace SharpEventBus.Subscriber;

/// <summary>
/// Abstract base class for implementing strongly-typed asynchronous event subscribers.
/// Automatically handles casting from the non-generic interface.
/// </summary>
/// <typeparam name="T">The type of event to handle.</typeparam>
public abstract class AsyncEventSubscriberBase<T> : IAsyncEventSubscriber<T> where T : IEvent
{
    /// <inheritdoc/>
    public abstract Task OnEventAsync(T e);

    /// <inheritdoc/>
    Task IAsyncEventSubscriber.OnEventAsync(IEvent e) => OnEventAsync((T)e);
}