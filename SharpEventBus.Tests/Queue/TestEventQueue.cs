using SharpEventBus.Event;
using SharpEventBus.Queue;

namespace SharpEventBus.Tests.Queue;

internal sealed class TestEventQueue : IEventQueue
{
    public readonly List<IEvent> EnqueuedEvents = [];
    
    private readonly Queue<IEvent> _queue = [];

    public bool IsEmpty => _queue.Count == 0;

    public void Enqueue(IEvent e)
    {
        EnqueuedEvents.Add(e);
        _queue.Enqueue(e);
    }

    public bool TryDequeue(out IEvent? e) => _queue.TryDequeue(out e);
}
