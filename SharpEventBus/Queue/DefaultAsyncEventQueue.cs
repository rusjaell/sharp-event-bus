using SharpEventBus.Event;
using System.Collections.Concurrent;

namespace SharpEventBus.Queue;

public sealed class DefaultAsyncEventQueue : IEventQueue
{
    internal DefaultAsyncEventQueue()
    {
    }

    private readonly ConcurrentQueue<IEvent> _queue = new ConcurrentQueue<IEvent>();

    public bool IsEmpty => _queue.IsEmpty;
    public int Count => _queue.Count;

    public void Enqueue(IEvent e)
    {
        ArgumentNullException.ThrowIfNull(e);
        _queue.Enqueue(e);
    }

    public bool TryDequeue(out IEvent? e) => _queue.TryDequeue(out e);

    public static DefaultAsyncEventQueue Default => new DefaultAsyncEventQueue();
}