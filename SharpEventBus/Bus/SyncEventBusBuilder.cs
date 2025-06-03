using SharpEventBus.Configuration;
using SharpEventBus.Dispatcher;
using SharpEventBus.Queue;

namespace SharpEventBus.Bus;

/// <summary>
/// Builder class for configuring and creating instances of <see cref="SyncEventBus"/>.
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
    /// <param name="configure">An optional action to configure the builder.</param>
    /// <returns>A configured <see cref="SyncEventBus"/> instance.</returns>
    public static SyncEventBus Create(Action<SyncEventBusBuilder>? configure = null)
    {
        var builder = new SyncEventBusBuilder();
        builder.WithEventQueueFactory(() => new DefaultEventQueue());
        builder.WithEventDispatcherFactory(() => new DefaultSyncEventDispatcher());
        builder.WithConfiguration(EventBusConfigurationBuilder.Create());

        configure?.Invoke(builder);

        return new SyncEventBus(builder._eventQueueFactory, builder._eventDispatcherFactory, builder._configuration!);
    }

    /// <summary>
    /// Specifies the factory method for the event queue implementation to be used.
    /// </summary>
    /// <param name="queueFactory">The factory that creates an <see cref="IEventQueue"/> instance.</param>
    /// <returns>The current <see cref="SyncEventBusBuilder"/> for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="queueFactory"/> is <c>null</c>.</exception>
    public SyncEventBusBuilder WithEventQueueFactory(Func<IEventQueue> queueFactory)
    {
        ArgumentNullException.ThrowIfNull(queueFactory);
        _eventQueueFactory = queueFactory;
        return this;
    }

    /// <summary>
    /// Specifies the factory method for the event dispatcher implementation to be used.
    /// </summary>
    /// <param name="dispatcherFactory">The factory that creates an <see cref="IEventDispatcher"/> instance.</param>
    /// <returns>The current <see cref="SyncEventBusBuilder"/> for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="dispatcherFactory"/> is <c>null</c>.</exception>
    public SyncEventBusBuilder WithEventDispatcherFactory(Func<IEventDispatcher> dispatcherFactory)
    {
        ArgumentNullException.ThrowIfNull(dispatcherFactory);
        _eventDispatcherFactory = dispatcherFactory;
        return this;
    }

    /// <summary>
    /// Configures the event bus using an action on <see cref="EventBusConfigurationBuilder"/>.
    /// </summary>
    /// <param name="builder">An action to configure the <see cref="EventBusConfigurationBuilder"/>.</param>
    /// <returns>The current <see cref="SyncEventBusBuilder"/> for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> is <c>null</c>.</exception>
    public SyncEventBusBuilder WithConfiguration(Action<EventBusConfigurationBuilder> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        _configuration = EventBusConfigurationBuilder.Create(builder);
        return this;
    }

    /// <summary>
    /// Specifies the <see cref="EventBusConfiguration"/> instance to use.
    /// </summary>
    /// <param name="configuration">The <see cref="EventBusConfiguration"/> instance.</param>
    /// <returns>The current <see cref="SyncEventBusBuilder"/> for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="configuration"/> is <c>null</c>.</exception>
    public SyncEventBusBuilder WithConfiguration(EventBusConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        _configuration = configuration;
        return this;
    }
}