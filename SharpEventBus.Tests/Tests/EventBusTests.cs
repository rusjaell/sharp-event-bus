using SharpEventBus.Bus;
using SharpEventBus.Configuration;
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
        var dispatcher = new DefaultSyncEventDispatcher();
        var queue = new DefaultSyncEventQueue();
        var config = EventBusConfigurationBuilder.Create();

        var queueException = Assert.Throws<ArgumentNullException>(() => new SyncEventBus(null, dispatcher, config));
        var dispatcherException = Assert.Throws<ArgumentNullException>(() => new SyncEventBus(queue, null, config));
        var configurationException = Assert.Throws<ArgumentNullException>(() => new SyncEventBus(queue, dispatcher, null));

        Assert.Equal("eventQueue", queueException.ParamName);
        Assert.Equal("eventDispatcher", dispatcherException.ParamName);
        Assert.Equal("configuration", configurationException.ParamName);
    }

    [Fact]
    public static void ConsumeEvents_ShouldProcessAllEnqueuedEvents()
    {
        var testQueue = new TestEventQueue();
        var testDispatcher = new TestEventDispatcher();

        var bus = SyncEventBusBuilder.Create(options =>
        {
            options.WithEventQueue(testQueue);
            options.WithEventDispatcher(testDispatcher);
        });

        var event1 = new TestEvent("event1");
        var event2 = new TestEvent("event2");

        testQueue.Enqueue(event1);
        testQueue.Enqueue(event2);

        bus.AddSubscriber(new TestSubscriber());

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

        var bus = SyncEventBusBuilder.Create(options =>
        {
            options.WithEventQueue(testQueue);
            options.WithEventDispatcher(testDispatcher);
        });

        var testEvent = new TestEvent("hello");
        var subscriber = new TestSubscriber();

        bus.AddSubscriber(subscriber);
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

        var bus = SyncEventBusBuilder.Create(options =>
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
        var bus = SyncEventBusBuilder.Create();
        var subscriber = new TestSubscriber();

        bus.AddSubscriber(subscriber);
        bus.PublishEvent(new TestEvent("test"));
        bus.ConsumeEvents();

        Assert.True(subscriber.Received);
    }

    [Fact]
    public static void ConsumeEvents_ShouldDispatchToSubscribers()
    {
        var testQueue = new TestEventQueue();
        var testDispatcher = new TestEventDispatcher();

        var bus = SyncEventBusBuilder.Create(options =>
        {
            options.WithEventQueue(testQueue);
            options.WithEventDispatcher(testDispatcher);
        });

        var testEvent = new TestEvent("test");

        testQueue.Enqueue(testEvent);
        bus.AddSubscriber(new TestSubscriber());
        bus.ConsumeEvents();

        Assert.Equal(1, testDispatcher.DispatchCallCount);
        Assert.Same(testEvent, testDispatcher.LastEvent);
    }
}
