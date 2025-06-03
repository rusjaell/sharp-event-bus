using SharpEventBus.Dispatcher;
using SharpEventBus.Event;
using SharpEventBus.Exceptions;
using SharpEventBus.Queue;
using SharpEventBus.Subscriber;
using System.Collections.Immutable;

namespace SharpEventBus.Consumer;

internal class DefaultAsyncEventConsumerWorker : IAsyncEventConsumer
{
    private readonly IEventQueue _queue;
    private readonly IAsyncEventDispatcher _dispatcher;
    private ImmutableArray<IAsyncEventSubscriber> _subscribers = ImmutableArray<IAsyncEventSubscriber>.Empty;

    private readonly SemaphoreSlim _queueSignal = new SemaphoreSlim(0);
    private readonly bool _debugLogging;
    private readonly int _maxConsumerConcurrency;
    private long _processedEvents;
    private long _completedTasks;

    internal DefaultAsyncEventConsumerWorker(IAsyncEventDispatcher dispatcher, IEventQueue queue, bool debugLogging, int maxConsumerConcurrency)
    {
        _dispatcher = dispatcher;
        _queue = queue;
        _debugLogging = debugLogging;
        _maxConsumerConcurrency = maxConsumerConcurrency;
    }

    public void AddAsyncSubscriber(IAsyncEventSubscriber subscriber)
    {
        ImmutableInterlocked.Update(ref _subscribers, list => list.Add(subscriber));
    }

    public void Enqueue(IEvent e)
    {
        _queue.Enqueue(e);
        _queueSignal.Release();
    }

    public async Task RunAsync(CancellationToken token)
    {
        if (_debugLogging)
            Console.WriteLine("[EventConsumer] Consumer started running");

        using var concurrencySemaphore = new SemaphoreSlim(_maxConsumerConcurrency);
        var runningTasks = new List<Task>();

        var monitorTask = Task.CompletedTask;
        if (_debugLogging)
            monitorTask = Task.Run(async () =>
            {
                try
                {
                    var lastCount = 0L;
                    var lastTime = DateTime.UtcNow;

                    var lastCompletedCount = 0L;

                    while (!token.IsCancellationRequested)
                    {
                        await Task.Delay(1000, token);

                        var currentProcessed = Interlocked.Read(ref _processedEvents);
                        var currentCompleted = Interlocked.Read(ref _completedTasks);

                        var now = DateTime.UtcNow;
                        var secondsElapsed = (now - lastTime).TotalSeconds;

                        var eventsPerSecond = (currentProcessed - lastCount) / secondsElapsed;
                        var completedPerSecond = (currentCompleted - lastCompletedCount) / secondsElapsed;

                        Console.WriteLine($"[EventConsumer] Subscribers: {_subscribers.Length} Queue: {_queue.Count}, Running Tasks: {runningTasks.Count}, Events/sec: {eventsPerSecond:0.##}, Completed Tasks/sec: {completedPerSecond:0.##}");

                        lastCount = currentProcessed;
                        lastCompletedCount = currentCompleted;
                        lastTime = now;
                    }
                }
                catch (OperationCanceledException)
                {
                }
            }, token);

        while (!token.IsCancellationRequested)
        {
            await _queueSignal.WaitAsync(token);

            while (_queue.TryDequeue(out var e))
            {
                if (e == null)
                    EventQueueTryDequeueException.Throw();

                if (runningTasks.Count >= _maxConsumerConcurrency)
                    await Task.WhenAny(runningTasks);

                await concurrencySemaphore.WaitAsync(token);
                Interlocked.Increment(ref _processedEvents);

                var dispatchTask = DispatchEventAsync(e, _subscribers, concurrencySemaphore, token);
                if (!dispatchTask.IsCompleted)
                    runningTasks.Add(dispatchTask);

                runningTasks.RemoveAll(t => t.IsCompleted);
            }
        }

        if (_debugLogging)
            Console.WriteLine("[EventConsumer] Consumer finishing tasks");

        await Task.WhenAll(runningTasks);

        if (_debugLogging)
        {
            try
            {
                await monitorTask;
            }
            catch (OperationCanceledException)
            {
            }

            Console.WriteLine("[EventConsumer] Consumer stopped running");
        }
    }

    private async Task DispatchEventAsync(IEvent e, IReadOnlyList<IAsyncEventSubscriber> subscribers, SemaphoreSlim concurrencySemaphore, CancellationToken token)
    {
        try
        {
            await _dispatcher.DispatchAsync(e, subscribers);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EventConsumer] Dispatch error: {ex}");
        }
        finally
        {
            concurrencySemaphore.Release();
            Interlocked.Increment(ref _completedTasks);
        }
    }
}
