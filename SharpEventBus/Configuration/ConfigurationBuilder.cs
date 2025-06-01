namespace SharpEventBus.Configuration;

/// <summary>
/// Builder for creating <see cref="EventBusConfiguration"/> instances with optional settings.
/// </summary>
public sealed class EventBusConfigurationBuilder
{
    private bool _debugLogging = false;

    private EventBusConfigurationBuilder() { }

    /// <summary>
    /// Creates a new <see cref="EventBusConfiguration"/> instance, optionally configuring it via the provided builder action.
    /// </summary>
    /// <param name="builder">An optional action to configure the builder.</param>
    /// <returns>A configured <see cref="EventBusConfiguration"/> instance.</returns>
    public static EventBusConfiguration Create(Action<EventBusConfigurationBuilder>? builder = null)
    {
        var configuration = new EventBusConfigurationBuilder();
        builder?.Invoke(configuration);

        return new EventBusConfiguration(configuration._debugLogging);
    }


    /// <summary>
    /// Enables debug logging in for the event bus.
    /// </summary>
    public EventBusConfigurationBuilder WithDebugLogging()
    {
        _debugLogging = true;
        return this;
    }
}