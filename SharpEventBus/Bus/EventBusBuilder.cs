using SharpEventBus.Dispatcher;
using SharpEventBus.Queue;

namespace SharpEventBus.Bus;

/// <summary>
/// Builder for configuring and creating instances of <see cref="EventBus"/>.
/// </summary>
public sealed class EventBusBuilder
{
    private IEventQueue? _eventQueue;
    private IEventDispatcher? _eventDispatcher;

    private EventBusBuilder() { }

    /// <summary>
    /// Creates a new <see cref="EventBus"/> instance with optional configuration.
    /// </summary>
    /// <param name="configure">An optional configuration action for the builder.</param>
    /// <returns>A configured <see cref="EventBus"/> instance.</returns>
    public static EventBus Create(Action<EventBusBuilder>? configure = null)
    {
        var builder = new EventBusBuilder();

        if (configure != null)
        {
            configure.Invoke(builder);
        }
        else
        {
            builder.WithEventQueue(new DefaultEventQueue());
            builder.WithEventDispatcher(new DefaultEventDispatcher());
        }
        return new EventBus(builder._eventQueue!, builder._eventDispatcher!);
    }

    /// <summary>
    /// Specifies the event queue to use in the <see cref="EventBus"/>.
    /// </summary>
    /// <param name="eventQueue">The event queue implementation.</param>
    /// <returns>The current <see cref="EventBusBuilder"/> for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="eventQueue"/> is <c>null</c>.</exception>
    public EventBusBuilder WithEventQueue(IEventQueue eventQueue)
    {
        ArgumentNullException.ThrowIfNull(eventQueue);
        _eventQueue = eventQueue;
        return this;
    }

    /// <summary>
    /// Specifies the event dispatcher to use in the <see cref="EventBus"/>.
    /// </summary>
    /// <param name="eventDispatcher">The event dispatcher implementation.</param>
    /// <returns>The current <see cref="EventBusBuilder"/> for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="eventDispatcher"/> is <c>null</c>.</exception>
    public EventBusBuilder WithEventDispatcher(IEventDispatcher eventDispatcher)
    {
        ArgumentNullException.ThrowIfNull(eventDispatcher);
        _eventDispatcher = eventDispatcher;
        return this;
    }
}
