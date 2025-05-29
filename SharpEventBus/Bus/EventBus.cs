using SharpEventBus.Dispatcher;
using SharpEventBus.Event;
using SharpEventBus.Exceptions;
using SharpEventBus.Queue;
using SharpEventBus.Subscriber;
using System.Runtime.InteropServices;

namespace SharpEventBus.Bus;

/// <summary>
/// An event bus implementation that processes events.
/// Events are enqueued and consumed in FIFO (first-in, first-out) order
/// by explicitly calling <see cref="ConsumeEvents"/> or <see cref="ConsumeOneEvent"/>.
/// </summary>
public sealed class EventBus : IEventBus
{
    private readonly Dictionary<Type, List<ISubscriber>> _subscribers = new();
    private readonly IEventQueue _eventQueue;
    private readonly IEventDispatcher _eventDispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventBus"/> class.
    /// </summary>
    /// <param name="eventQueue">The event queue used to store published events.</param>
    /// <param name="eventDispatcher">The dispatcher responsible for invoking subscribers.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="eventQueue"/> or <paramref name="eventDispatcher"/> is <c>null</c>.
    /// </exception>
    internal EventBus(IEventQueue? eventQueue, IEventDispatcher? eventDispatcher)
    {
        ArgumentNullException.ThrowIfNull(eventQueue);
        ArgumentNullException.ThrowIfNull(eventDispatcher);

        _eventQueue = eventQueue;
        _eventDispatcher = eventDispatcher;
    }

    /// <summary>
    /// Publishes an event to the bus, adding it to the event queue.
    /// </summary>
    /// <typeparam name="T">The event type.</typeparam>
    /// <param name="e">The event instance to publish.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="e"/> is null.</exception>
    public void PublishEvent<T>(T e) where T : IEvent
    {
        ArgumentNullException.ThrowIfNull(e);
        _eventQueue.Enqueue(e);
    }

    /// <summary>
    /// Subscribes a handler to events of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The event type to subscribe to.</typeparam>
    /// <param name="subscriber">The subscriber that will handle the event.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="subscriber"/> is null.</exception>
    public void Subscribe<T>(ISubscriber<T> subscriber) where T : IEvent
    {
        ArgumentNullException.ThrowIfNull(subscriber);

        var type = typeof(T);

        ref var list = ref CollectionsMarshal.GetValueRefOrAddDefault(_subscribers, type, out var exists);
        if (!exists || list == null)
            list = new List<ISubscriber>();

        list.Add(subscriber);
    }

    /// <summary>
    /// Consumes and dispatches all events currently queued.
    /// This method blocks until the queue is empty.
    /// </summary>
    /// <exception cref="EventQueueTryDequeueException">Thrown if a null event is dequeued.</exception>
    public void ConsumeEvents()
    {
        while (_eventQueue.TryDequeue(out var e))
        {
            if (e == null)
                EventQueueTryDequeueException.Throw();

            var type = e!.GetType();
            if (_subscribers.TryGetValue(type, out var handlers))
                _eventDispatcher.Dispatch(e, handlers);
        }
    }

    /// <summary>
    /// Attempts to consume and dispatch a single event from the queue.
    /// </summary>
    /// <returns>
    /// <c>true</c> if an event was dequeued and dispatched; 
    /// otherwise, <c>false</c> if the queue was empty.
    /// </returns>
    /// <exception cref="EventQueueTryDequeueException">Thrown if a null event is dequeued.</exception>
    public bool ConsumeOneEvent()
    {
        if (_eventQueue.TryDequeue(out var e))
        {
            if (e == null)
                EventQueueTryDequeueException.Throw();

            var type = e!.GetType();
            if (_subscribers.TryGetValue(type, out var handlers))
                _eventDispatcher.Dispatch(e, handlers);
            return true;
        }
        return false;
    }
}