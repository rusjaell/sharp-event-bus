using SharpEventBus.Event;
using SharpEventBus.Subscriber;

namespace SharpEventBus.Consumer;

public interface IAsyncEventConsumer
{
    void Enqueue(IEvent e);
    void AddAsyncSubscriber(IAsyncEventSubscriber subscriber);
    Task RunAsync(CancellationToken token);
}