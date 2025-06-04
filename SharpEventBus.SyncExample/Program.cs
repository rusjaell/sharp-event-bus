using SharpEventBus.Bus;
using SharpEventBus.Configuration;
using SharpEventBus.SyncExample.Events;
using SharpEventBus.SyncExample.Subscribers;

namespace SharpEventBus.SyncExample;

internal sealed class Program
{
    public static async Task Main(string[] args)
    {
        // Manual Configuration
        var configuration = EventBusConfigurationBuilder.Create(builder =>
        {
            builder.WithDebugLogging();
        });

        // custom configuration
        var eventBus = SyncEventBusBuilder.Create(options =>
        {
            // with a manual builder
            options.WithConfiguration(configuration);

            // with a builder
            //options.WithConfiguration(builder =>
            //{
            //});
        });

        // Create a default EventBus
        //var eventBus = EventBusBuilder.Create();

        // Create a custom-implemented EventBus
        //var eventBus = SyncEventBusBuilder.Create(options =>
        //{
        //    // With a custom event queue
        //    c

        //    // Or a custom event dispatcher
        //    options.WithEventDispatcherFactory(() => new CustomEventDispatcher());
        //});

        // Manually Create and add a subscriber
        //var orderPlacedSubscriber = eventBus.AddSubscriber(new OrderPlacedSubscriber());
        //var orderCancelledSubscriber = eventBus.AddSubscriber(new OrderCancelledSubscriber());

        // Automatically Create and add a subscriber
        var orderPlacedSubscriber = eventBus.RegisterSubscriber<OrderPlacedSubscriber, OrderPlacedEvent>();
        var orderCancelledSubscriber = eventBus.RegisterSubscriber<OrderCancelledSubscriber, OrderCancelledEvent>();

        // Setup cancellation token source to allow graceful shutdown
        using var cts = new CancellationTokenSource();

        // Listen for Ctrl+C key press to trigger cancellation
        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            Console.WriteLine("Cancellation requested, stopping...");

            cts.Cancel();

            eventArgs.Cancel = true;
        };

        Console.WriteLine("Starting event publishing loop. Press Ctrl+C to stop.");

        try
        {
            // Main loop runs until cancellation is requested
            while (!cts.Token.IsCancellationRequested)
            {
                PublishRandomOrderEvent(eventBus);

                // Processes all queued events synchronously
                eventBus.ConsumeEvents();

                await Task.Delay(Random.Shared.Next(500, 1500), cts.Token);
            }
        }
        catch (TaskCanceledException)
        {
        }

        Console.WriteLine("Event publishing loop stopped.");
    }

    // Simulates some kind of Event Publish
    private static void PublishRandomOrderEvent(SyncEventBus eventBus)
    {
        var random = Random.Shared;

        var chance = random.NextDouble();
        if (chance <= 0.5)
            return;

        var orderId = Guid.NewGuid().ToString();

        var wasOrderPlaced = random.NextDouble() > 0.5;
        if (wasOrderPlaced)
        {
            eventBus.Publish(new OrderPlacedEvent(orderId, DateTime.UtcNow));
            return;
        }

        eventBus.Publish(new OrderCancelledEvent(orderId, "Customer Request"));
    }
}
