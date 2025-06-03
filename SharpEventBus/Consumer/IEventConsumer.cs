using SharpEventBus.Event;
using SharpEventBus.Subscriber;

namespace SharpEventBus.Consumer;

public interface IEventConsumer
{
    void Enqueue(IEvent e);
    void AddSubscriber(IEventSubscriber subscriber);
    void ConsumeEvents();
}
