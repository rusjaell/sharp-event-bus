using SharpEventBus.Event;

namespace SharpEventBus.Subscriber;

/// <summary>
/// Represents an asynchronous subscriber that can handle events.
/// </summary>
public interface IAsyncEventSubscriber
{
    /// <summary>
    /// Handles the specified event asynchronously.
    /// </summary>
    /// <param name="e">The event to handle.</param>
    Task OnEventAsync(IEvent e);
}

/// <summary>
/// Represents a strongly-typed asynchronous subscriber for events of type <typeparamref name="TEvent"/>.
/// </summary>
/// <typeparam name="TEvent">The type of event to handle.</typeparam>
public interface IAsyncEventSubscriber<TEvent> : IAsyncEventSubscriber where TEvent : IEvent
{
    /// <summary>
    /// Handles the event of type <typeparamref name="TEvent"/> asynchronously.
    /// </summary>
    /// <param name="e">The event to handle.</param>
    Task OnEventAsync(TEvent e);
}