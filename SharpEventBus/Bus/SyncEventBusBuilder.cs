using SharpEventBus.Configuration;
using SharpEventBus.Dispatcher;
using SharpEventBus.Queue;

namespace SharpEventBus.Bus;

/// <summary>
/// Builder for configuring and creating instances of <see cref="SyncEventBus"/>.
/// </summary>
public sealed class SyncEventBusBuilder
{
    private IEventQueue? _eventQueue;
    private IEventDispatcher? _eventDispatcher;
    private EventBusConfiguration? _configuration;

    private SyncEventBusBuilder() { }

    /// <summary>
    /// Creates a new <see cref="SyncEventBus"/> instance with optional configuration.
    /// </summary>
    /// <param name="configure">An optional configuration action for the builder.</param>
    /// <returns>A configured <see cref="SyncEventBus"/> instance.</returns>
    public static SyncEventBus Create(Action<SyncEventBusBuilder>? configure = null)
    {
        var builder = new SyncEventBusBuilder();
        builder.WithEventQueue(new DefaultSyncEventQueue());
        builder.WithEventDispatcher(new DefaultSyncEventDispatcher());
        builder.WithConfiguration(EventBusConfigurationBuilder.Create());

        configure?.Invoke(builder);

        return new SyncEventBus(builder._eventQueue!, builder._eventDispatcher!, builder._configuration!);
    }

    /// <summary>
    /// Specifies the event queue to use in the <see cref="SyncEventBus"/>.
    /// </summary>
    /// <param name="eventQueue">The event queue implementation.</param>
    /// <returns>The current <see cref="SyncEventBusBuilder"/> for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="eventQueue"/> is <c>null</c>.</exception>
    public SyncEventBusBuilder WithEventQueue(IEventQueue eventQueue)
    {
        ArgumentNullException.ThrowIfNull(eventQueue);
        _eventQueue = eventQueue;
        return this;
    }

    /// <summary>
    /// Specifies the event dispatcher to use in the <see cref="SyncEventBus"/>.
    /// </summary>
    /// <param name="eventDispatcher">The event dispatcher implementation.</param>
    /// <returns>The current <see cref="SyncEventBusBuilder"/> for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="eventDispatcher"/> is <c>null</c>.</exception>
    public SyncEventBusBuilder WithEventDispatcher(IEventDispatcher eventDispatcher)
    {
        ArgumentNullException.ThrowIfNull(eventDispatcher);
        _eventDispatcher = eventDispatcher;
        return this;
    }

    /// <summary>
    /// Sets the <see cref="EventBusConfiguration"/> to use in the <see cref="SyncEventBus"/>.
    /// </summary>
    /// <param name="builder">The event bus configuration instance.</param>
    /// <returns>The current <see cref="SyncEventBusBuilder"/> for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> is <c>null</c>.</exception>
    public SyncEventBusBuilder WithConfiguration(Action<EventBusConfigurationBuilder> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        _configuration = EventBusConfigurationBuilder.Create(builder);
        return this;
    }

    /// <summary>
    /// Sets the <see cref="EventBusConfiguration"/> to use in the <see cref="SyncEventBus"/>.
    /// </summary>
    /// <param name="configuration">The event bus configuration instance.</param>
    /// <returns>The current <see cref="SyncEventBusBuilder"/> for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="configuration"/> is <c>null</c>.</exception>
    public SyncEventBusBuilder WithConfiguration(EventBusConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        _configuration = configuration;
        return this;
    }
}
