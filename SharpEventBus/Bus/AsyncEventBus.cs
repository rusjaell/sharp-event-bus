using SharpEventBus.Configuration;
using SharpEventBus.Dispatcher;
using SharpEventBus.Event;
using SharpEventBus.Exceptions;
using SharpEventBus.Queue;
using SharpEventBus.Subscriber;
using System.Runtime.InteropServices;

namespace SharpEventBus.Bus;

public sealed class AsyncEventBus : IAsyncEventBus
{
    private readonly Dictionary<Type, List<IAsyncEventSubscriber>> _subscribers = [];

    private readonly IEventQueue _eventQueue;
    private readonly IAsyncEventDispatcher _eventDispatcher;
    private readonly EventBusConfiguration _configuration;

    private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);
    private CancellationTokenSource? _cts;
    private Task? _consumerTask;

    public AsyncEventBus(IEventQueue? eventQueue, IAsyncEventDispatcher? eventDispatcher, EventBusConfiguration? configuration)
    {
        ArgumentNullException.ThrowIfNull(eventQueue);
        ArgumentNullException.ThrowIfNull(eventDispatcher);
        ArgumentNullException.ThrowIfNull(configuration);

        _eventQueue = eventQueue;
        _eventDispatcher = eventDispatcher;
        _configuration = configuration;
    }

    public void Publish<T>(T e) where T : class, IEvent
    {
        if (_configuration.DebugLogging)
            Console.WriteLine($"[EventBus] PublishEvent: {e.GetType().Name}");

        ArgumentNullException.ThrowIfNull(e);
        _eventQueue.Enqueue(e);

        _signal.Release();
    }

    public void AddAsyncSubscriber<T>(IAsyncEventSubscriber<T> subscriber) where T : class, IEvent
    {
        ArgumentNullException.ThrowIfNull(subscriber);

        var type = typeof(T);

        lock (_subscribers)
        {
            ref var list = ref CollectionsMarshal.GetValueRefOrAddDefault(_subscribers, type, out var exists);
            if (!exists || list == null)
                list = [];

            list.Add(subscriber);

            if (_configuration.DebugLogging)
                Console.WriteLine($"[EventBus] Subscribe: {type.Name} now has {list.Count} subscribers");
        }
    }

    public void StartConsuming()
    {
        if (_consumerTask != null)
            throw new InvalidOperationException("Already started.");

        _cts = new CancellationTokenSource();
        _consumerTask = Task.Run(() => TestConsumeLoopAsync(_cts.Token));
    }

    private async Task TestConsumeLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                if (_configuration.DebugLogging)
                    Console.WriteLine("[EventBus] Waiting for signal...");

                await _signal.WaitAsync(token);

                if (_configuration.DebugLogging)
                    Console.WriteLine("[EventBus] Signal received, processing events...");
            }
            catch (OperationCanceledException)
            {
                if (_configuration.DebugLogging)
                    Console.WriteLine("[EventBus] ConsumeLoop canceled.");
                break;
            }

            while (_eventQueue.TryDequeue(out var e))
            {
                if (e == null)
                    EventQueueTryDequeueException.Throw();
                
                var type = e.GetType();

                List<IAsyncEventSubscriber>? handlers;
                lock (_subscribers)
                {
                    handlers = _subscribers.GetValueOrDefault(type);
                }

                if (handlers == null)
                {
                    if (_configuration.DebugLogging)
                        Console.WriteLine($"[EventBus] DispatchEvent: No subscribers for {type.Name}");
                    continue;
                }

                try
                {
                    if (_configuration.DebugLogging)
                        Console.WriteLine($"[EventBus] Dispatching: {type.Name}");

                    await _eventDispatcher.DispatchAsync(e, handlers.AsReadOnly());

                    if (_configuration.DebugLogging)
                        Console.WriteLine($"[EventBus] Event dispatched: {type.Name}");
                }
                catch (Exception ex)
                {
                    if (_configuration.DebugLogging)
                        Console.WriteLine($"[EventBus] Dispatch error: {ex}");
                }
            }

            if (_configuration.DebugLogging)
                Console.WriteLine("[EventBus] Finished processing all queued events.");
        }

        if (_configuration.DebugLogging)
            Console.WriteLine("[EventBus] Exiting consume loop.");
    }

    public async Task StopConsumingAsync()
    {
        if (_consumerTask == null || _cts == null)
            return;

        _cts.Cancel();
        _signal.Release();

        await _consumerTask;

        _consumerTask = null;
        _cts.Dispose();
        _cts = null;
    }

    void IAsyncEventBus.AddAsyncSubscriber<T>(IAsyncEventSubscriber<T> subscriber) => throw new NotImplementedException();
}