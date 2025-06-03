using SharpEventBus.Event;

namespace SharpEventBus.SyncExample.Events;

// Event representing a order being placed
public record class OrderPlacedEvent(string OrderId, DateTime Timestamp) : IEvent;