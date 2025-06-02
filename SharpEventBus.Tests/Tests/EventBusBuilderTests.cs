using SharpEventBus.Bus;
using SharpEventBus.Tests.Event;

namespace SharpEventBus.Tests.Tests;

public static class EventBusBuilderTests
{
    [Fact]
    public static void EventBusBuilder_Create_WithEventQueue_ShouldThrowArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => SyncEventBusBuilder.Create(options => options.WithEventQueue(null!)));

        Assert.Equal("eventQueue", exception.ParamName);
    }

    [Fact]
    public static void EventBusBuilder_Create_WithDispatcher_ShouldThrowArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => SyncEventBusBuilder.Create(options => options.WithEventDispatcher(null!)));

        Assert.Equal("eventDispatcher", exception.ParamName);
    }

    [Fact]
    public static void PublishTestEvent_NullEvent_ShouldThrowArgumentNullException()
    {
        var bus = SyncEventBusBuilder.Create();

        Assert.Throws<ArgumentNullException>(() => bus.PublishEvent<TestEvent>(null!));
    }
}