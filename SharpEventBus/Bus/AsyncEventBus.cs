using SharpEventBus.Configuration;
using SharpEventBus.Consumer;
using SharpEventBus.Dispatcher;
using SharpEventBus.Event;
using SharpEventBus.Queue;
using SharpEventBus.Subscriber;

namespace SharpEventBus.Bus;

/// <summary>
/// Thread-Safe Asynchronous implementation of the <see cref="IAsyncEventBus"/> interface.
/// Manages event publishing, subscriber registration, and event consumption using internal consumers.
/// </summary>
public sealed class AsyncEventBus : IAsyncEventBus
{
    private readonly Func<IEventQueue> _queueFactory;
    private readonly Func<IAsyncEventDispatcher> _dispatcherFactory;
    private readonly EventBusConfiguration _configuration;
    private readonly Dictionary<Type, IAsyncEventConsumer> _consumers = [];

    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private readonly List<Task> _consumerTasks = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncEventBus"/> class with specified factories and configuration.
    /// </summary>
    /// <param name="queueFactory">Factory method to create event queues.</param>
    /// <param name="dispatcherFactory">Factory method to create event dispatchers.</param>
    /// <param name="configuration">Configuration options for the event bus.</param>
    internal AsyncEventBus(Func<IEventQueue>? queueFactory, Func<IAsyncEventDispatcher>? dispatcherFactory, EventBusConfiguration? configuration)
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
        ArgumentNullException.ThrowIfNull(e);

        IAsyncEventConsumer? consumer;
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
    public void AddSubscriber<T>(IAsyncEventSubscriber<T> subscriber) where T : class, IEvent
    {
        ArgumentNullException.ThrowIfNull(subscriber);

        if (_configuration.DebugLogging)
            Console.WriteLine($"[EventBus] Adding subscriber {subscriber.GetType().Name} for {typeof(T).Name}");

        var consumer = GetOrCreateConsumer<T>();
        consumer.AddSubscriber(subscriber);
    }

    /// <summary>
    /// Retrieves or creates a dedicated <see cref="IAsyncEventConsumer"/> for the given event type <typeparamref name="T"/>.
    /// Ensures that only one consumer exists per event type. The method is thread-safe.
    /// </summary>
    /// <typeparam name="T">The type of event to get or create a consumer for. Must implement <see cref="IEvent"/>.</typeparam>
    /// <returns>The <see cref="IAsyncEventConsumer"/> associated with the specified event type.</returns>
    private IAsyncEventConsumer GetOrCreateConsumer<T>() where T : class, IEvent
    {
        var type = typeof(T);

        lock (_consumers)
        {
            if (_consumers.TryGetValue(type, out var consumer))
                return consumer;
        }

        var queue = _queueFactory.Invoke();
        var dispatcher = _dispatcherFactory.Invoke();

        var newConsumer = new DefaultAsyncEventConsumer(dispatcher, queue, _configuration);

        if (_configuration.DebugLogging)
            Console.WriteLine($"[EventBus] Created consumer for {type.Name}");

        lock (_consumers)
        {
            if (_consumers.TryGetValue(type, out var consumer))
                return consumer;

            _consumers.Add(type, newConsumer);
            _consumerTasks.Add(newConsumer.RunAsync(_cts.Token));
        }
        return newConsumer;
    }

    /// <summary>
    /// Initiates a graceful shutdown of the event bus by cancelling all running consumers
    /// and awaiting their completion. Safe to call multiple times.
    /// </summary>
    /// <returns>A task that completes when all consumers have finished processing or cancellation has propagated.</returns>
    public async Task ShutdownAsync()
    {
        _cts.Cancel();

        try
        {
            await Task.WhenAll(_consumerTasks).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }

        if (_configuration.DebugLogging)
            Console.WriteLine("[EventBus] Shutdown complete");
    }
}