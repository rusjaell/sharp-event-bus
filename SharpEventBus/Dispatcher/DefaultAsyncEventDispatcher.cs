using SharpEventBus.Event;
using SharpEventBus.Subscriber;

namespace SharpEventBus.Dispatcher;

/// <summary>
/// Default implementation of <see cref="IAsyncEventDispatcher"/> that
/// asynchronously dispatches events to subscribers by invoking the <see cref="IAsyncEventSubscriber.OnEventAsync"/> method concurrently.
/// </summary>
public sealed class DefaultAsyncEventDispatcher : IAsyncEventDispatcher
{
    internal DefaultAsyncEventDispatcher()
    {
    }

    /// <inheritdoc />
    public async Task DispatchAsync(IEvent e, IReadOnlyList<IAsyncEventSubscriber> subscribers)
    {
        ArgumentNullException.ThrowIfNull(e);

        var tasksArray = new Task[subscribers.Count];
        for (int i = 0; i < subscribers.Count; i++)
            tasksArray[i] = subscribers[i].OnEventAsync(e);
        await Task.WhenAll(tasksArray).ConfigureAwait(false);
    }
}
