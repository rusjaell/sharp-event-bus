using SharpEventBus.Bus;
using SharpEventBus.Example.Events;
using SharpEventBus.Example.Subscribers;

namespace SharpEventBus.Example;

internal sealed class Program
{
    public static async Task Main(string[] args)
    {
        // Create an default EventBus
        var eventBus = EventBusBuilder.Create();

        // Create a custom-implemented EventBus
        //var eventBus = EventBusBuilder.Create(options =>
        //{
        //    // With a custom event queue
        //    options.WithEventQueue(new CustomEventQueue());
        //
        //    // Or a custom event dispatcher
        //    options.WithEventDispatcher(new CustomEventDispatcher());
        //});

        // Creates a Subscriber for the OrderPlacedEvent events
        eventBus.Subscribe(new OrderPlacedSubscriber());
        eventBus.Subscribe(new OrderCancelledSubscriber());

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

                // Alternative is to manually process events
                //for(var i = 0; i < 2; i++)
                //    eventBus.ConsumeOneEvent();

                await Task.Delay(Random.Shared.Next(500, 1500), cts.Token);
            }
        }
        catch (TaskCanceledException)
        {
        }

        Console.WriteLine("Event publishing loop stopped.");
    }

    // Simulates some kind of Event Publish
    private static void PublishRandomOrderEvent(EventBus eventBus)
    {
        var random = Random.Shared;

        var chance = random.NextDouble();
        if (chance <= 0.5)
            return;

        var orderId = Guid.NewGuid().ToString();

        var wasOrderPlaced = random.NextDouble() > 0.5;
        if (wasOrderPlaced)
        {
            eventBus.PublishEvent(new OrderPlacedEvent(orderId, DateTime.UtcNow));
            return;
        }

        eventBus.PublishEvent(new OrderCancelledEvent(orderId, "Customer Request"));
    }
}
