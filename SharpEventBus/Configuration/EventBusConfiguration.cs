using SharpEventBus.Bus;

namespace SharpEventBus.Configuration;

/// <summary>
/// Configuration for <see cref="EventBus"/> instances.
/// </summary>
public sealed class EventBusConfiguration
{
    public readonly bool DebugLogging;

    internal EventBusConfiguration(bool debugLogging)
    {
        DebugLogging = debugLogging;
    }
}