namespace SharpEventBus.Configuration;

/// <summary>
/// Builder for creating <see cref="EventBusConfiguration"/> instances with custom settings.
/// </summary>
public sealed class EventBusConfigurationBuilder
{

    private EventBusConfigurationBuilder() { }

    /// <summary>
    /// Creates a new <see cref="EventBusConfiguration"/> instance.
    /// </summary>
    /// <param name="builder">An optional action to configure the builder.</param>
    /// <returns>A configured <see cref="EventBusConfiguration"/> instance.</returns>
    public static EventBusConfiguration Create(Action<EventBusConfigurationBuilder>? builder = null)
    {
        var configuration = new EventBusConfigurationBuilder();
        builder?.Invoke(configuration);

        return new EventBusConfiguration();
    }
}