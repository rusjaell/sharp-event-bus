using SharpEventBus.Configuration;
using SharpEventBus.Dispatcher;
using SharpEventBus.Queue;

namespace SharpEventBus.Bus;

/// <summary>
/// Builder for creating and configuring instances of <see cref="AsyncEventBus"/>.
/// Provides methods to customize the event queue, dispatcher, and configuration.
/// </summary>
public sealed class AsyncEventBusBuilder
{
    private Func<IEventQueue>? _queueFactory;
    private Func<IAsyncEventDispatcher>? _dispatcherFactory;
    private EventBusConfiguration? _configuration;

    private AsyncEventBusBuilder() { }

    /// <summary>
    /// Creates a new instance of <see cref="AsyncEventBus"/> using optional custom configuration.
    /// </summary>
    /// <param name="configure">An optional configuration action to customize the builder.</param>
    /// <returns>A configured <see cref="AsyncEventBus"/> instance.</returns>
    public static AsyncEventBus Create(Action<AsyncEventBusBuilder>? configure = null)
    {
        var builder = new AsyncEventBusBuilder();
        builder.WithEventQueueFactory(() => new DefaultEventQueue());
        builder.WithEventDispatcherFactory(() => new DefaultAsyncEventDispatcher());
        builder.WithConfiguration(EventBusConfigurationBuilder.Create());

        configure?.Invoke(builder);

        return new AsyncEventBus(builder._queueFactory, builder._dispatcherFactory, builder._configuration);
    }

    /// <summary>
    /// Sets the factory method to create the event queue instance.
    /// </summary>
    /// <param name="queueFactory">Factory function returning an <see cref="IEventQueue"/> instance.</param>
    /// <returns>The current <see cref="AsyncEventBusBuilder"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="queueFactory"/> is <c>null</c>.</exception>
    public AsyncEventBusBuilder WithEventQueueFactory(Func<IEventQueue> queueFactory)
    {
        ArgumentNullException.ThrowIfNull(queueFactory);
        _queueFactory = queueFactory;
        return this;
    }

    /// <summary>
    /// Sets the factory method to create the asynchronous event dispatcher instance.
    /// </summary>
    /// <param name="dispatcherFactory">Factory function returning an <see cref="IAsyncEventDispatcher"/> instance.</param>
    /// <returns>The current <see cref="AsyncEventBusBuilder"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="dispatcherFactory"/> is <c>null</c>.</exception>
    public AsyncEventBusBuilder WithEventDispatcherFactory(Func<IAsyncEventDispatcher> dispatcherFactory)
    {
        ArgumentNullException.ThrowIfNull(dispatcherFactory);
        _dispatcherFactory = dispatcherFactory;
        return this;
    }

    /// <summary>
    /// Sets the configuration instance for the event bus.
    /// </summary>
    /// <param name="configuration">The <see cref="EventBusConfiguration"/> to use.</param>
    /// <returns>The current <see cref="AsyncEventBusBuilder"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="configuration"/> is <c>null</c>.</exception>
    public AsyncEventBusBuilder WithConfiguration(EventBusConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        _configuration = configuration;
        return this;
    }
}
