using SharpEventBus.Event;
using SharpEventBus.Queue;

namespace SharpEventBus.Tests.Queue;

internal sealed class TestEventQueue : IEventQueue
{
    private readonly Queue<IEvent> _queue = [];
    
    public readonly List<IEvent> EnqueuedEvents = [];

    public bool IsEmpty => _queue.Count == 0;
    public int Count => _queue.Count;

    public void Enqueue(IEvent e)
    {
        EnqueuedEvents.Add(e);
        _queue.Enqueue(e);
    }

    public bool TryDequeue(out IEvent? e) => _queue.TryDequeue(out e);
}
