using SharpEventBus.AsyncExample.Events;
using SharpEventBus.AsyncExample.Subscribers;
using SharpEventBus.Bus;
using SharpEventBus.Configuration;

namespace SharpEventBus.AsyncExample;

internal sealed class Program
{
    public static async Task Main(string[] args)
    {
        var configuration = EventBusConfigurationBuilder.Create(builder =>
        {
            builder.WithDebugLogging();
            builder.WithMaxConsumerConcurrency(16);
        });

        var asyncEventBus = AsyncEventBusBuilder.Create(builder =>
        {
            builder.WithConfiguration(configuration);
        });

        asyncEventBus.AddSubscriber(new OrderPlacedAsyncSubscriber());
        asyncEventBus.AddSubscriber(new OrderCancelledAsyncSubscriber());

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
            while (!cts.Token.IsCancellationRequested)
            {
                PublishRandomOrderEvent(asyncEventBus);
                await Task.Delay(Random.Shared.Next(50, 500), cts.Token);
            }
        }
        catch (TaskCanceledException)
        {
        }

        Console.WriteLine("Event publishing loop stopped.");
    }

    // Simulates some kind of Event Publish
    private static void PublishRandomOrderEvent(AsyncEventBus asyncEventBus)
    {
        var random = Random.Shared;

        var chance = random.NextDouble();
        if (chance <= 0.5)
            return;

        var orderId = Guid.NewGuid().ToString();

        var count = random.Next(10, 100);

        var wasOrderPlaced = random.NextDouble() > 0.5;
        if (wasOrderPlaced)
        {
            for (var i = 0; i < count; i++)
                asyncEventBus.Publish(new OrderPlacedEvent(orderId, DateTime.UtcNow));
            return;
        }

        for (var i = 0; i < count; i++)
            asyncEventBus.Publish(new OrderCancelledEvent(orderId, "Customer Request"));
    }
}
