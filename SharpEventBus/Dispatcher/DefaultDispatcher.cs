using SharpEventBus.Event;
using SharpEventBus.Subscriber;

namespace SharpEventBus.Dispatcher;

/// <summary>
/// Default implementation of <see cref="IEventDispatcher"/> that synchronously
/// dispatches events to subscribers by invoking their <see cref="ISubscriber.OnEvent"/> method.
/// </summary>
internal sealed class DefaultEventDispatcher : IEventDispatcher
{
    /// <inheritdoc />
    public void Dispatch(IEvent e, IEnumerable<ISubscriber> subscribers)
    {
        ArgumentNullException.ThrowIfNull(e);
        ArgumentNullException.ThrowIfNull(subscribers);
        
        foreach (var handler in subscribers)
            handler.OnEvent(e);
    }
}
