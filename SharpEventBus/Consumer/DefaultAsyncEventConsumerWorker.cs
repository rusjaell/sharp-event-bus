using SharpEventBus.Configuration;
using SharpEventBus.Dispatcher;
using SharpEventBus.Event;
using SharpEventBus.Exceptions;
using SharpEventBus.Queue;
using SharpEventBus.Subscriber;
using System.Collections.Immutable;

namespace SharpEventBus.Consumer;

/// <summary>
/// Thread-safe default implementation of <see cref="IAsyncEventConsumer"/> that
/// processes events concurrently with a maximum concurrency limit, manages subscribers, and optionally logs diagnostic information.
/// </summary>
internal class DefaultAsyncEventConsumer : IAsyncEventConsumer
{
    private readonly IEventQueue _queue;
    private readonly IAsyncEventDispatcher _dispatcher;
    private ImmutableArray<IAsyncEventSubscriber> _subscribers = [];

    private readonly SemaphoreSlim _queueSignal = new SemaphoreSlim(0);
    private readonly bool _debugLogging;
    private readonly int _maxConsumerConcurrency;
    private long _processedEvents;
    private long _completedTasks;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultAsyncEventConsumer"/> class
    /// using the specified dispatcher, event queue, and configuration settings.
    /// </summary>
    /// <param name="dispatcher">The asynchronous event dispatcher used to invoke subscribers.</param>
    /// <param name="queue">The event queue used to store and retrieve events.</param>
    /// <param name="configuration">The configuration containing debug logging and concurrency options.</param>
    internal DefaultAsyncEventConsumer(IAsyncEventDispatcher dispatcher, IEventQueue queue, EventBusConfiguration configuration)
    {
        _dispatcher = dispatcher;
        _queue = queue;
        _debugLogging = configuration.DebugLogging;
        _maxConsumerConcurrency = configuration.MaxConsumerConcurrency;
    }

    /// <inheritdoc/>
    public void Enqueue(IEvent e)
    {
        _queue.Enqueue(e);
        _queueSignal.Release();
    }

    /// <inheritdoc/>
    public void AddSubscriber(IAsyncEventSubscriber subscriber)
    {
        ImmutableInterlocked.Update(ref _subscribers, list => list.Add(subscriber));
    }

    /// <summary>
    /// Runs the consumer asynchronously until cancellation is requested,
    /// processing events with concurrency limits and optional debug logging.
    /// </summary>
    /// <param name="token">Cancellation token to stop the consumer.</param>
    /// <returns>A task representing the lifetime of the running consumer.</returns>
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
                        await Task.Delay(1000, token).ConfigureAwait(false);

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
            await _queueSignal.WaitAsync(token).ConfigureAwait(false);


            while (_queue.TryDequeue(out var e))
            {
                if (e == null)
                    EventQueueTryDequeueException.Throw();

                if (runningTasks.Count >= _maxConsumerConcurrency)
                    await Task.WhenAny(runningTasks).ConfigureAwait(false);

                await concurrencySemaphore.WaitAsync(token).ConfigureAwait(false);
                Interlocked.Increment(ref _processedEvents);

                var dispatchTask = DispatchEventAsync(e, _subscribers, concurrencySemaphore, token);
                if (!dispatchTask.IsCompleted)
                    runningTasks.Add(dispatchTask);

                runningTasks.RemoveAll(t => t.IsCompleted);
            }
        }

        if (_debugLogging)
            Console.WriteLine("[EventConsumer] Consumer finishing tasks");

        await Task.WhenAll(runningTasks).ConfigureAwait(false);

        if (_debugLogging)
        {
            try
            {
                await monitorTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }

            Console.WriteLine("[EventConsumer] Consumer stopped running");
        }
    }

    /// <summary>
    /// Dispatches the event to subscribers asynchronously, handling exceptions,
    /// and releases the concurrency semaphore once done.
    /// </summary>
    /// <param name="e">The event to dispatch.</param>
    /// <param name="subscribers">The list of subscribers to notify.</param>
    /// <param name="concurrencySemaphore">Semaphore controlling concurrency.</param>
    /// <param name="token">Cancellation token.</param>
    private async Task DispatchEventAsync(IEvent e, IReadOnlyList<IAsyncEventSubscriber> subscribers, SemaphoreSlim concurrencySemaphore, CancellationToken token)
    {
        try
        {
                await _dispatcher.DispatchAsync(e, subscribers).ConfigureAwait(false);
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
