using SharpEventBus.Event;
using SharpEventBus.Subscriber;

namespace SharpEventBus.Dispatcher;

/// <summary>
/// Defines an asynchronous dispatcher responsible for delivering events to subscribers.
/// </summary>
public interface IAsyncEventDispatcher
{
    /// <summary>
    /// Asynchronously dispatches the specified event to the provided subscribers.
    /// </summary>
    /// <param name="e">The event to dispatch.</param>
    /// <param name="subscribers">The subscribers that will receive the event.</param>
    Task DispatchAsync(IEvent e, IReadOnlyList<IAsyncEventSubscriber> subscribers);
}