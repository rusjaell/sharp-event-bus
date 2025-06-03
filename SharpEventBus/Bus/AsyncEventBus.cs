using SharpEventBus.Configuration;
using SharpEventBus.Consumer;
using SharpEventBus.Dispatcher;
using SharpEventBus.Event;
using SharpEventBus.Queue;
using SharpEventBus.Subscriber;

namespace SharpEventBus.Bus;

public sealed class AsyncEventBus : IAsyncEventBus
{
    private readonly Func<IEventQueue> _queueFactory;
    private readonly Func<IAsyncEventDispatcher> _dispatcherFactory;
    private readonly Dictionary<Type, IAsyncEventConsumer> _consumers = [];
    private readonly EventBusConfiguration _configuration;

    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private readonly List<Task> _consumerTasks = [];

    internal AsyncEventBus(Func<IEventQueue>? queueFactory, Func<IAsyncEventDispatcher>? dispatcherFactory, EventBusConfiguration? configuration)
    {
        ArgumentNullException.ThrowIfNull(queueFactory);
        ArgumentNullException.ThrowIfNull(dispatcherFactory);
        ArgumentNullException.ThrowIfNull(configuration);

        _queueFactory = queueFactory;
        _dispatcherFactory = dispatcherFactory;
        _configuration = configuration;
    }

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

    public void AddSubscriber<T>(IAsyncEventSubscriber<T> subscriber) where T : class, IEvent
    {
        ArgumentNullException.ThrowIfNull(subscriber);

        if (_configuration.DebugLogging)
            Console.WriteLine($"[EventBus] Adding subscriber {subscriber.GetType().Name} for {typeof(T).Name}");

        var consumer = GetOrCreateConsumer<T>();
        consumer.AddAsyncSubscriber(subscriber);
    }

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

        var newConsumer = new DefaultAsyncEventConsumerWorker(dispatcher, queue, _configuration.DebugLogging, _configuration.MaxConsumerConcurrency);

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

    public async Task ShutdownAsync()
    {
        _cts.Cancel();

        try
        {
            await Task.WhenAll(_consumerTasks);
        }
        catch (OperationCanceledException)
        {
        }

        if (_configuration.DebugLogging)
            Console.WriteLine("[EventBus] Shutdown complete");
    }
}