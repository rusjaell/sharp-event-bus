using SharpEventBus.Event;

namespace SharpEventBus.Subscriber
{
    /// <summary>
    /// Defines a subscriber that handles events.
    /// </summary>
    public interface ISubscriber
    {
        /// <summary>
        /// Handles the received event.
        /// </summary>
        /// <param name="e">The event to handle.</param>
        void OnEvent(IEvent e);
    }

    /// <summary>
    /// Defines a subscriber that handles events of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of event this subscriber handles.</typeparam>
    public interface ISubscriber<T> : ISubscriber where T : IEvent
    {
        /// <summary>
        /// Handles the received event of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="e">The event to handle.</param>
        void OnEvent(T e);
    }

    /// <summary>
    /// Base class for subscribers that handle events of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of event to handle.</typeparam>
    public abstract class SubscriberBase<T> : ISubscriber<T> where T : IEvent
    {
        /// <summary>
        /// Handles the event of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="e">The event to handle.</param>
        public abstract void OnEvent(T e);

        /// <summary>
        /// Handles the event using the non-generic interface by casting the event to type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="e">The event to handle.</param>
        void ISubscriber.OnEvent(IEvent e) => OnEvent((T)e);
    }
}
