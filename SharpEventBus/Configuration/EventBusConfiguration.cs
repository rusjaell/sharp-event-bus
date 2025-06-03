using SharpEventBus.Bus;

namespace SharpEventBus.Configuration;

/// <summary>
/// Configuration for <see cref="SyncEventBus"/> instances.
/// </summary>
public sealed class EventBusConfiguration
{
    public readonly bool DebugLogging;
    public readonly int MaxConsumerConcurrency;

    internal EventBusConfiguration(bool debugLogging, int maxConsumerConcurrency)
    {
        DebugLogging = debugLogging;
        MaxConsumerConcurrency = maxConsumerConcurrency;
    }
}