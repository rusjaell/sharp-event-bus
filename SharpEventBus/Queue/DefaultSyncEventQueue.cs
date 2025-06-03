using SharpEventBus.Event;
using System.Collections.Concurrent;

namespace SharpEventBus.Queue;

/// <summary>
/// Thread-Safe Default implementation of <see cref="IEventQueue"/> using a FIFO queue.
/// </summary>
public sealed class DefaultEventQueue : IEventQueue
{
    internal DefaultEventQueue()
    {
    }

    private readonly ConcurrentQueue<IEvent> _queue = new ConcurrentQueue<IEvent>();

    /// <inheritdoc />
    public bool IsEmpty => _queue.IsEmpty;

    /// <inheritdoc />
    public int Count => _queue.Count;

    /// <inheritdoc />
    public void Enqueue(IEvent e)
    {
        ArgumentNullException.ThrowIfNull(e);
        _queue.Enqueue(e);
    }

    /// <inheritdoc />
    public bool TryDequeue(out IEvent? e) => _queue.TryDequeue(out e);
}