using SharpEventBus.Event;
using SharpEventBus.Subscriber;

namespace SharpEventBus.Bus;

public interface IAsyncEventBus
{
    void Publish<T>(T e) where T : class, IEvent;
    void AddSubscriber<T>(IAsyncEventSubscriber<T> subscriber) where T : class, IEvent;
}