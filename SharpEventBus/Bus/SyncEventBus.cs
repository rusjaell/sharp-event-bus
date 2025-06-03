using SharpEventBus.Configuration;
using SharpEventBus.Consumer;
using SharpEventBus.Dispatcher;
using SharpEventBus.Event;
using SharpEventBus.Queue;
using SharpEventBus.Subscriber;

namespace SharpEventBus.Bus;

/// <summary>
/// Thread-Safe Synchronous implementation of the <see cref="IEventBus"/> interface.
/// Manages event publishing, subscriber registration, and event consumption using internal consumers.
/// </summary>
public sealed class SyncEventBus : IEventBus
{
    private readonly Func<IEventQueue> _queueFactory;
    private readonly Func<IEventDispatcher> _dispatcherFactory;
    private readonly EventBusConfiguration _configuration;
    private readonly Dictionary<Type, IEventConsumer> _consumers = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="SyncEventBus"/> class with specified factories and configuration.
    /// </summary>
    /// <param name="queueFactory">Factory method to create event queues.</param>
    /// <param name="dispatcherFactory">Factory method to create event dispatchers.</param>
    /// <param name="configuration">Configuration options for the event bus.</param>
    internal SyncEventBus(Func<IEventQueue>? queueFactory, Func<IEventDispatcher>? dispatcherFactory, EventBusConfiguration? configuration)
    {
        ArgumentNullException.ThrowIfNull(queueFactory);
        ArgumentNullException.ThrowIfNull(dispatcherFactory);
        ArgumentNullException.ThrowIfNull(configuration);

        _queueFactory = queueFactory;
        _dispatcherFactory = dispatcherFactory;
        _configuration = configuration;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="e"/> is null.</exception>
    public void Publish<T>(T e) where T : class, IEvent
    {
        if (_configuration.DebugLogging)
            Console.WriteLine($"[EventBus] PublishEvent: {e.GetType().Name}");

        ArgumentNullException.ThrowIfNull(e);

        IEventConsumer? consumer;
        lock (_consumers)
            consumer = _consumers.GetValueOrDefault(typeof(T));

        if (consumer == null)
        {
            if (_configuration.DebugLogging)
                Console.WriteLine($"[EventBus] No consumer found for event type {typeof(T).Name}");
            return;
        }

        consumer.Enqueue(e);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="subscriber"/> is null.</exception>
    public void AddSubscriber<T>(IEventSubscriber<T> subscriber) where T : class, IEvent
    {
        ArgumentNullException.ThrowIfNull(subscriber);

        if (_configuration.DebugLogging)
            Console.WriteLine($"[EventBus] Adding subscriber {subscriber.GetType().Name} for {typeof(T).Name}");

        var consumer = GetOrCreateConsumer<T>();
        consumer.AddSubscriber(subscriber);
    }

    /// <inheritdoc/>
    public void ConsumeEvents()
    {
        lock (_consumers)
        {
            foreach(var consumer in _consumers.Values)
                consumer.ConsumeEvents();
        }
    }

    /// <summary>
    /// Retrieves an existing consumer for the specified event type or creates a new one if none exists.
    /// </summary>
    /// <typeparam name="T">The type of event the consumer will handle.</typeparam>
    /// <returns>The event consumer for the specified event type.</returns>
    private IEventConsumer GetOrCreateConsumer<T>() where T : class, IEvent
    {
        var type = typeof(T);

        lock (_consumers)
        {
            if (_consumers.TryGetValue(type, out var consumer))
                return consumer;
        }

        var queue = _queueFactory.Invoke();
        var dispatcher = _dispatcherFactory.Invoke();

        var newConsumer = new DefaultEventConsumer(dispatcher, queue, _configuration.DebugLogging);

        if (_configuration.DebugLogging)
            Console.WriteLine($"[EventBus] Created consumer for {type.Name}");

        lock (_consumers)
        {
            if (_consumers.TryGetValue(type, out var consumer))
                return consumer;

            _consumers.Add(type, newConsumer);
        }
        return newConsumer;
    }
}