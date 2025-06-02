using SharpEventBus.Bus;

namespace SharpEventBus.Configuration;

/// <summary>
/// Configuration for <see cref="SyncEventBus"/> instances.
/// </summary>
public sealed class EventBusConfiguration
{
    public readonly bool DebugLogging;

    internal EventBusConfiguration(bool debugLogging)
    {
        DebugLogging = debugLogging;
    }
}