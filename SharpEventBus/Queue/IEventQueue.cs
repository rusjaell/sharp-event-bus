using SharpEventBus.Event;

namespace SharpEventBus.Queue;

/// <summary>
/// Represents a queue for storing and retrieving events.
/// </summary>
public interface IEventQueue
{
    /// <summary>
    /// Gets a value indicating whether the event queue is empty.
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Enqueues an event into the queue.
    /// </summary>
    /// <param name="e">The event to enqueue.</param>
    void Enqueue(IEvent e);

    /// <summary>
    /// Attempts to dequeue an event from the queue.
    /// </summary>
    /// <param name="e">When this method returns, contains the dequeued event if successful; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if an event was dequeued successfully; otherwise, <c>false</c>.</returns>
    bool TryDequeue(out IEvent? e);
}