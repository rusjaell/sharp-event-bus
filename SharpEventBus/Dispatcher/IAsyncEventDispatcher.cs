using SharpEventBus.Event;
using SharpEventBus.Subscriber;

namespace SharpEventBus.Dispatcher;

public interface IAsyncEventDispatcher
{
    Task DispatchAsync(IEvent e, IReadOnlyList<IAsyncEventSubscriber> subscribers);
}