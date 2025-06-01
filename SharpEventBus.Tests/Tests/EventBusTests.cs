using SharpEventBus.Bus;
using SharpEventBus.Dispatcher;
using SharpEventBus.Queue;
using SharpEventBus.Tests.Dispatcher;
using SharpEventBus.Tests.Event;
using SharpEventBus.Tests.Queue;
using SharpEventBus.Tests.Subscriber;

namespace SharpEventBus.Tests.Tests;

public static class EventBusTests
{
    [Fact]
    public static void Constructor_ShouldThrowArgumentNullException()
    {
        var queueException = Assert.Throws<ArgumentNullException>(() => new EventBus(null, new DefaultEventDispatcher(), null));
        var dispatcherException = Assert.Throws<ArgumentNullException>(() => new EventBus(new DefaultEventQueue(), null, null));

        Assert.Equal("eventQueue", queueException.ParamName);
        Assert.Equal("eventDispatcher", dispatcherException.ParamName);
    }

    [Fact]
    public static void ConsumeEvents_ShouldProcessAllEnqueuedEvents()
    {
        var testQueue = new TestEventQueue();
        var testDispatcher = new TestEventDispatcher();

        var bus = EventBusBuilder.Create(options =>
        {
            options.WithEventQueue(testQueue);
            options.WithEventDispatcher(testDispatcher);
        });

        var event1 = new TestEvent("event1");
        var event2 = new TestEvent("event2");

        testQueue.Enqueue(event1);
        testQueue.Enqueue(event2);

        bus.Subscribe(new TestSubscriber());

        bus.ConsumeEvents();

        Assert.Equal(2, testDispatcher.DispatchCallCount);
        Assert.Contains(event1, testDispatcher.DispatchedEvents);
        Assert.Contains(event2, testDispatcher.DispatchedEvents);
    }

    [Fact]
    public static void ConsumeEvents_ShouldPassCorrectSubscribersToDispatcher()
    {
        var testQueue = new TestEventQueue();
        var testDispatcher = new TestEventDispatcher();

        var bus = EventBusBuilder.Create(options =>
        {
            options.WithEventQueue(testQueue);
            options.WithEventDispatcher(testDispatcher);
        });

        var testEvent = new TestEvent("hello");
        var subscriber = new TestSubscriber();

        bus.Subscribe(subscriber);
        testQueue.Enqueue(testEvent);

        bus.ConsumeEvents();

        Assert.NotNull(testDispatcher.LastSubscribers);
        Assert.Contains(testDispatcher.LastSubscribers!, s => s.GetType() == typeof(TestSubscriber));
    }

    [Fact]
    public static void PublishEvent_ShouldEnqueueEvent()
    {
        var testQueue = new TestEventQueue();
        var testDispatcher = new TestEventDispatcher();

        var bus = EventBusBuilder.Create(options =>
        {
            options.WithEventQueue(testQueue);
            options.WithEventDispatcher(testDispatcher);
        });

        var testEvent = new TestEvent("test");

        bus.PublishEvent(testEvent);

        Assert.Single(testQueue.EnqueuedEvents);
        Assert.Equal(testEvent, testQueue.EnqueuedEvents[0]);
    }

    [Fact]
    public static void Subscribe_ShouldRegisterSubscriber()
    {
        var bus = EventBusBuilder.Create();
        var subscriber = new TestSubscriber();

        bus.Subscribe(subscriber);
        bus.PublishEvent(new TestEvent("test"));
        bus.ConsumeEvents();

        Assert.True(subscriber.Received);
    }

    [Fact]
    public static void ConsumeEvents_ShouldDispatchToSubscribers()
    {
        var testQueue = new TestEventQueue();
        var testDispatcher = new TestEventDispatcher();

        var bus = EventBusBuilder.Create(options =>
        {
            options.WithEventQueue(testQueue);
            options.WithEventDispatcher(testDispatcher);
        });

        var testEvent = new TestEvent("test");

        testQueue.Enqueue(testEvent);
        bus.Subscribe(new TestSubscriber());
        bus.ConsumeEvents();

        Assert.Equal(1, testDispatcher.DispatchCallCount);
        Assert.Same(testEvent, testDispatcher.LastEvent);
    }
}
