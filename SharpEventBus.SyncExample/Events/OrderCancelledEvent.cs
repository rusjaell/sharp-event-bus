﻿using SharpEventBus.Event;

namespace SharpEventBus.SyncExample.Events;

// Event representing a order being cancelled
public record class OrderCancelledEvent(string OrderId, string Reason) : IEvent;
