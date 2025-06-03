using SharpEventBus.Dispatcher;
using SharpEventBus.Event;
using SharpEventBus.Exceptions;
using SharpEventBus.Queue;
using SharpEventBus.Subscriber;
using System.Runtime.InteropServices;

namespace SharpEventBus.Consumer;

/// <summary>
/// Default implementation of <see cref="IEventConsumer"/> that processes events synchronously.
/// It manages an event queue, a list of subscribers, and dispatches events to subscribers.
/// </summary>
internal class DefaultEventConsumer : IEventConsumer
{
    private readonly IEventQueue _queue;
    private readonly IEventDispatcher _dispatcher;
    private List<IEventSubscriber> _subscribers = [];

    private readonly bool _debugLogging;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultEventConsumer"/> class
    /// with the specified dispatcher, event queue, and debug logging option.
    /// </summary>
    /// <param name="dispatcher">The dispatcher responsible for delivering events to subscribers.</param>
    /// <param name="queue">The queue used to store events before processing.</param>
    /// <param name="debugLogging">If <c>true</c>, enables debug logging output.</param>
    internal DefaultEventConsumer(IEventDispatcher dispatcher, IEventQueue queue, bool debugLogging)
    {
        _dispatcher = dispatcher;
        _queue = queue;
        _debugLogging = debugLogging;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Adds a subscriber to the internal list to receive dispatched events.
    /// </remarks>
    public void AddSubscriber(IEventSubscriber subscriber)
    {
        _subscribers.Add(subscriber);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Enqueues an event to be consumed later by <see cref="ConsumeEvents"/>.
    /// </remarks>
    public void Enqueue(IEvent e)
    {
        _queue.Enqueue(e);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Processes all queued events synchronously. For each event, it dispatches
    /// the event to all registered subscribers. Throws <see cref="EventQueueTryDequeueException"/>
    /// if a dequeued event is unexpectedly null.
    /// Debug logging outputs detailed information during consumption and dispatch.
    /// </remarks>
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
