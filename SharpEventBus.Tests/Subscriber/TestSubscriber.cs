using SharpEventBus.Subscriber;
using SharpEventBus.Tests.Event;

namespace SharpEventBus.Tests.Subscriber;

internal sealed class TestSubscriber : SyncEventSubscriberBase<TestEvent>
{
    public bool Received { get; private set; }

    public override void OnEvent(TestEvent e)
    {
        Received = true;
    }
}
