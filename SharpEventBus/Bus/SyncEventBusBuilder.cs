using SharpEventBus.Configuration;
using SharpEventBus.Dispatcher;
using SharpEventBus.Queue;

namespace SharpEventBus.Bus;

/// <summary>
/// Builder for configuring and creating instances of <see cref="SyncEventBus"/>.
/// </summary>
public sealed class SyncEventBusBuilder
{
    private Func<IEventQueue>? _eventQueueFactory;
    private Func<IEventDispatcher>? _eventDispatcherFactory;
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
        builder.WithEventQueueFactory(() => new DefaultSyncEventQueue());
        builder.WithEventDispatcherFactory(() => new DefaultSyncEventDispatcher());
        builder.WithConfiguration(EventBusConfigurationBuilder.Create());

        configure?.Invoke(builder);

        return new SyncEventBus(builder._eventQueueFactory, builder._eventDispatcherFactory, builder._configuration!);
    }

    /// <summary>
    /// Specifies the event queue to use in the <see cref="SyncEventBus"/>.
    /// </summary>
    /// <param name="factory">The event queue implementation.</param>
    /// <returns>The current <see cref="SyncEventBusBuilder"/> for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="factory"/> is <c>null</c>.</exception>
    public SyncEventBusBuilder WithEventQueueFactory(Func<IEventQueue> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        _eventQueueFactory = factory;
        return this;
    }

    /// <summary>
    /// Specifies the event dispatcher to use in the <see cref="SyncEventBus"/>.
    /// </summary>
    /// <param name="factory">The event dispatcher implementation.</param>
    /// <returns>The current <see cref="SyncEventBusBuilder"/> for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="factory"/> is <c>null</c>.</exception>
    public SyncEventBusBuilder WithEventDispatcherFactory(Func<IEventDispatcher> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        _eventDispatcherFactory = factory;
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