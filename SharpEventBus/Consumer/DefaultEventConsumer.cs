using SharpEventBus.Dispatcher;
using SharpEventBus.Event;
using SharpEventBus.Exceptions;
using SharpEventBus.Queue;
using SharpEventBus.Subscriber;
using System.Runtime.InteropServices;

namespace SharpEventBus.Consumer;

internal class DefaultEventConsumer : IEventConsumer
{
    private readonly IEventQueue _queue;
    private readonly IEventDispatcher _dispatcher;
    private List<IEventSubscriber> _subscribers = [];

    private readonly bool _debugLogging;

    internal DefaultEventConsumer(IEventDispatcher dispatcher, IEventQueue queue, bool debugLogging)
    {
        _dispatcher = dispatcher;
        _queue = queue;
        _debugLogging = debugLogging;
    }

    public void AddSubscriber(IEventSubscriber subscriber)
    {
        _subscribers.Add(subscriber);
    }

    public void Enqueue(IEvent e)
    {
        _queue.Enqueue(e);
    }

    public void ConsumeEvents()
    {
        if (_queue.IsEmpty)
            return;

        if (_debugLogging)
            Console.WriteLine("[EventConsumer] Consuming Events");

        while (_queue.TryDequeue(out var e))
        {
            if (e == null)
                EventQueueTryDequeueException.Throw();

            if (_debugLogging)
                Console.WriteLine($"[EventBus] DispatchEvent: {e.GetType().Name}");

            var span = CollectionsMarshal.AsSpan(_subscribers);
            _dispatcher.Dispatch(e, in span);
        }
    }
}
