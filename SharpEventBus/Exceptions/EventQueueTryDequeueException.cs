using SharpEventBus.Event;
using SharpEventBus.Queue;
using System.Diagnostics.CodeAnalysis;

namespace SharpEventBus.Exceptions;

/// <summary>
/// Exception thrown when the event queue's <see cref="IEventQueue.TryDequeue(out IEvent?)"/> method 
/// returns a null event, which should never occur.
/// </summary>
internal class EventQueueTryDequeueException : InvalidOperationException
{
    public EventQueueTryDequeueException()
       : base("The event queue indicated a successful dequeue operation but returned a null event.\n\nThis violates the expected result of the event queue and likely indicates a bug the event queue implementation.")
    {
    }

    [DoesNotReturn] internal static void Throw() => throw new EventQueueTryDequeueException();
}