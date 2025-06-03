using SharpEventBus.Event;
using SharpEventBus.Subscriber;

namespace SharpEventBus.Dispatcher;

/// <summary>
/// Default implementation of <see cref="IEventDispatcher"/> that synchronously
/// dispatches events to subscribers by invoking their <see cref="IEventSubscriber.OnEvent"/> method.
/// </summary>
internal sealed class DefaultSyncEventDispatcher : IEventDispatcher
{
    /// <inheritdoc />
    public void Dispatch(IEvent e, in Span<IEventSubscriber> subscribers)
    {
        ArgumentNullException.ThrowIfNull(e);
        for (var i = 0; i < subscribers.Length; i++)
            subscribers[i].OnEvent(e);
    }
}
