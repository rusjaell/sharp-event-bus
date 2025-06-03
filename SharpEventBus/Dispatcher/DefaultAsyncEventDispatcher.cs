using SharpEventBus.Event;
using SharpEventBus.Subscriber;

namespace SharpEventBus.Dispatcher;

public sealed class DefaultAsyncEventDispatcher : IAsyncEventDispatcher
{
    internal DefaultAsyncEventDispatcher()
    {
    }

    public async Task DispatchAsync(IEvent e, IReadOnlyList<IAsyncEventSubscriber> subscribers)
    {
        var tasksArray = new Task[subscribers.Count];
        for (int i = 0; i < subscribers.Count; i++)
            tasksArray[i] = subscribers[i].OnEventAsync(e);
        await Task.WhenAll(tasksArray);
    }

    public static DefaultAsyncEventDispatcher Default => new DefaultAsyncEventDispatcher();
}
