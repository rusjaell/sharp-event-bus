using SharpEventBus.Event;

namespace SharpEventBus.Subscriber;

public interface IAsyncEventSubscriber
{
    Task OnEventAsync(IEvent e);
}

public interface IAsyncEventSubscriber<T> : IAsyncEventSubscriber where T : IEvent
{
    Task OnEventAsync(T e);
}

public abstract class AsyncEventSubscriberBase<T> : IAsyncEventSubscriber<T> where T : IEvent
{
    public abstract Task OnEventAsync(T e);

    Task IAsyncEventSubscriber.OnEventAsync(IEvent e) => OnEventAsync((T)e);
}
