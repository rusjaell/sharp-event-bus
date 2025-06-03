using SharpEventBus.Event;

namespace SharpEventBus.AsyncExample.Events;

// Event representing a order being placed
public record class OrderPlacedEvent(string OrderId, DateTime Timestamp) : IEvent;