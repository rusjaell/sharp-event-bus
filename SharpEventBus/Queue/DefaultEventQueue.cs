using SharpEventBus.Event;

namespace SharpEventBus.Queue;

/// <summary>
/// Default implementation of <see cref="IEventQueue"/> using a FIFO queue.
/// </summary>
internal sealed class DefaultEventQueue : IEventQueue
{
    private readonly Queue<IEvent> _queue = new Queue<IEvent>();

    /// <inheritdoc />
    public bool IsEmpty => _queue.Count == 0;

    /// <inheritdoc />
    public void Enqueue(IEvent e)
    {
        ArgumentNullException.ThrowIfNull(e);
        _queue.Enqueue(e);
    }

    /// <inheritdoc />
    public bool TryDequeue(out IEvent? e) => _queue.TryDequeue(out e);
}
