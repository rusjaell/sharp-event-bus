using SharpEventBus.Configuration;
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
/// by explicitly calling <see cref="ConsumeWithLimit"/> or <see cref="ConsumeOneEvent"/>.
/// </summary>
public sealed class EventBus : IEventBus
{
    private readonly Dictionary<Type, List<ISubscriber>> _subscribers = new();
    private readonly IEventQueue _eventQueue;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly EventBusConfiguration _configuration;

    /// <summary>
    /// Constructs a new instance of the <see cref="EventBus"/> class using the specified event queue, dispatcher, and configuration.
    /// </summary>
    /// <param name="eventQueue">The queue used to store and manage published events.</param>
    /// <param name="eventDispatcher">The component responsible for dispatching events to subscribers.</param>
    /// <param name="configuration">The configuration settings for the event bus.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="eventQueue"/>, <paramref name="eventDispatcher"/>, or <paramref name="configuration"/> is <c>null</c>.
    /// </exception>
    internal EventBus(IEventQueue? eventQueue, IEventDispatcher? eventDispatcher, EventBusConfiguration? configuration)
    {
        ArgumentNullException.ThrowIfNull(eventQueue);
        ArgumentNullException.ThrowIfNull(eventDispatcher);
        ArgumentNullException.ThrowIfNull(configuration);

        _eventQueue = eventQueue;
        _eventDispatcher = eventDispatcher;
        _configuration = configuration;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="e"/> is null.</exception>
    public void PublishEvent<T>(T e) where T : class, IEvent
    {
        ArgumentNullException.ThrowIfNull(e);
        _eventQueue.Enqueue(e);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="subscriber"/> is null.</exception>
    public void Subscribe<T>(ISubscriber<T> subscriber) where T : class, IEvent
    {
        ArgumentNullException.ThrowIfNull(subscriber);

        var type = typeof(T);

        ref var list = ref CollectionsMarshal.GetValueRefOrAddDefault(_subscribers, type, out var exists);
        if (!exists || list == null)
            list = [];

        list.Add(subscriber);
    }

    /// <inheritdoc/>
    /// <exception cref="EventQueueTryDequeueException">Thrown if a null event is unexpectedly dequeued.</exception>
    public void ConsumeEvents()
    {
        while (_eventQueue.TryDequeue(out var e))
        {
            if (e == null)
                EventQueueTryDequeueException.Throw();
            DispatchEvent(e);
        }
    }

    /// <inheritdoc/>
    /// <exception cref="EventQueueTryDequeueException">Thrown if a null event is unexpectedly dequeued.</exception>
    public bool ConsumeOneEvent()
    {
        if (_eventQueue.TryDequeue(out var e))
        {
            if (e == null)
                EventQueueTryDequeueException.Throw();
            DispatchEvent(e);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Dispatches the specified event to all registered handlers.
    /// </summary>
    /// <param name="e">The event to dispatch.</param>
    internal void DispatchEvent(IEvent e)
    {
        var type = e.GetType();
        if (!_subscribers.TryGetValue(type, out var handlers))
            return;
        
        var span = CollectionsMarshal.AsSpan(handlers);
        _eventDispatcher.Dispatch(e, in span);
    }
}