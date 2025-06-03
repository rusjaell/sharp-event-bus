using SharpEventBus.Configuration;
using SharpEventBus.Dispatcher;
using SharpEventBus.Queue;

namespace SharpEventBus.Bus;

public sealed class AsyncEventBusBuilder
{
    private Func<IEventQueue>? _queueFactory;
    private Func<IAsyncEventDispatcher>? _dispatcherFactory;
    private EventBusConfiguration? _configuration;

    private AsyncEventBusBuilder() { }

    public static AsyncEventBus Create(Action<AsyncEventBusBuilder>? configure = null)
    {
        var builder = new AsyncEventBusBuilder();
        builder.WithEventQueueFactory(() => new DefaultAsyncEventQueue());
        builder.WithEventDispatcherFactory(() => new DefaultAsyncEventDispatcher());
        builder.WithConfiguration(EventBusConfigurationBuilder.Create());

        configure?.Invoke(builder);

        return new AsyncEventBus(builder._queueFactory, builder._dispatcherFactory, builder._configuration);
    }

    public AsyncEventBusBuilder WithEventQueueFactory(Func<IEventQueue> queueFactory)
    {
        ArgumentNullException.ThrowIfNull(queueFactory);
        _queueFactory = queueFactory;
        return this;
    }

    public AsyncEventBusBuilder WithEventDispatcherFactory(Func<IAsyncEventDispatcher> dispatcherFactory)
    {
        ArgumentNullException.ThrowIfNull(dispatcherFactory);
        _dispatcherFactory = dispatcherFactory;
        return this;
    }

    public AsyncEventBusBuilder WithConfiguration(EventBusConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        _configuration = configuration;
        return this;
    }
}