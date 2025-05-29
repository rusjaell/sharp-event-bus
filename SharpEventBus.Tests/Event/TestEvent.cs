using SharpEventBus.Event;

namespace SharpEventBus.Tests.Event;

internal record class TestEvent(string Message) : IEvent;
