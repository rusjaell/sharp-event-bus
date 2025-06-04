using SharpEventBus.Event;
using SharpEventBus.Subscriber;

namespace SharpEventBus.Bus;

/// <summary>
/// Defines a framework for an event bus that supports publishing events, subscribing to events, and consumption of queued events.
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publishes an event to the bus.
    /// </summary>
    /// <typeparam name="TEvent">The type of event being published.</typeparam>
    /// <param name="e">The event instance to publish.</param>
    void Publish<TEvent>(TEvent e) where TEvent : class, IEvent;

    /// <summary>
    /// Manually subscribes a handler instance for a specific event.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to subscribe to.</typeparam>
    /// <param name="subscriber">The subscriber instance that will handle the event.</param>
    /// <returns>The created and registered subscriber instance.</returns>
    IEventSubscriber<TEvent> AddSubscriber<TEvent>(IEventSubscriber<TEvent> subscriber) where TEvent : class, IEvent;

    /// <summary>
    /// Creates and registers a subscriber of type <typeparamref name="TSubscriber"/> for events of type <typeparamref name="TEvent"/>.
    /// The event bus instantiates the subscriber internally using the provided constructor arguments (if any).
    /// Use this method for simpler subscriber registration without manual instantiation.
    /// </summary>
    /// <typeparam name="TSubscriber">The subscriber type, implementing <see cref="IEventSubscriber{TEvent}"/>.</typeparam>
    /// <typeparam name="TEvent">The event type that the subscriber handles, implementing <see cref="IEvent"/>.</typeparam>
    /// <param name="args">
    /// Optional constructor arguments passed to <typeparamref name="TSubscriber"/>.
    /// If none are provided, the parameterless constructor will be used.
    /// </param>
    /// <returns>The created and registered subscriber instance.</returns>
    TSubscriber RegisterSubscriber<TSubscriber, TEvent>(params object[] args) 
        where TSubscriber : class, IEventSubscriber<TEvent>
        where TEvent : class, IEvent;

    /// <summary>
    /// Processes all queued events.
    /// </summary>
    void ConsumeEvents();
}