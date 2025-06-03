using SharpEventBus.Event;
using SharpEventBus.Subscriber;

namespace SharpEventBus.Dispatcher;

/// <summary>
/// Defines a dispatcher responsible for delivering events to subscribers.
/// </summary>
public interface IEventDispatcher
{
    /// <summary>
    /// Dispatches the specified event to the provided subscribers.
    /// </summary>
    /// <param name="e">The event to dispatch.</param>
    /// <param name="subscribers">The subscribers to receive the event.</param>
    void Dispatch(IEvent e, in Span<IEventSubscriber> subscribers);
}