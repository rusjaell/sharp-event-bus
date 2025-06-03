using SharpEventBus.Event;
using SharpEventBus.Subscriber;

namespace SharpEventBus.Consumer;

/// <summary>
/// Represents an asynchronous event consumer that enqueues events, manages subscribers, and processes events asynchronously.
/// </summary>
public interface IAsyncEventConsumer
{
    /// <summary>
    /// Enqueues an event for asynchronous processing.
    /// </summary>
    /// <param name="e">The event to enqueue.</param>
    void Enqueue(IEvent e);

    /// <summary>
    /// Adds a subscriber to receive dispatched events.
    /// </summary>
    /// <param name="subscriber">The subscriber to add.</param>
    void AddSubscriber(IAsyncEventSubscriber subscriber);

    /// <summary>
    /// Runs the event consumer asynchronously, processing queued events until cancellation is requested.
    /// </summary>
    /// <param name="token">Cancellation token to stop processing.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RunAsync(CancellationToken token);
}