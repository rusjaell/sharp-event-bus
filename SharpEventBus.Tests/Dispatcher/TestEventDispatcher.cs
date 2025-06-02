using SharpEventBus.Dispatcher;
using SharpEventBus.Event;
using SharpEventBus.Subscriber;

namespace SharpEventBus.Tests.Dispatcher;

internal sealed class TestEventDispatcher : IEventDispatcher
{
    public int DispatchCallCount { get; private set; }
    public IEvent? LastEvent { get; private set; }
    public IEventSubscriber[]? LastSubscribers { get; private set; }
    public List<IEvent> DispatchedEvents { get; } = [];

    public void Dispatch(IEvent e, in Span<IEventSubscriber> subscribers)
    {
        DispatchCallCount++;
        LastEvent = e;
        LastSubscribers = subscribers.ToArray();
        DispatchedEvents.Add(e);
    }
}