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
    public void Dispatch(IEvent e, in Span<ISubscriber> subscribers)
    {
        ArgumentNullException.ThrowIfNull(e);

        for (var i = 0; i < subscribers.Length; i++)
            subscribers[i].OnEvent(e);
    }
}
