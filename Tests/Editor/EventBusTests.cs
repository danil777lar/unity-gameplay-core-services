using System;
using Larje.Core.Events;
using NUnit.Framework;

namespace Larje.Core.Tests
{
    public sealed class EventBusTests
    {
        [Test]
        public void SubscriberReceivesEventByRuntimeType()
        {
            EventBus bus = new EventBus();
            int received = 0;
            bus.Subscribe<TestEvent>(x => received += x.Amount);

            GameEvent asBase = new TestEvent(3);
            bus.Publish(asBase);

            Assert.AreEqual(3, received);
        }

        [Test]
        public void DisposedSubscriptionStopsReceiving()
        {
            EventBus bus = new EventBus();
            int received = 0;
            IDisposable subscription = bus.Subscribe<TestEvent>(x => received++);
            bus.Publish(new TestEvent(1));
            subscription.Dispose();
            bus.Publish(new TestEvent(1));

            Assert.AreEqual(1, received);
        }

        [Test]
        public void UnsubscribeDuringPublishIsSafe()
        {
            EventBus bus = new EventBus();
            int received = 0;
            Action<TestEvent> handler = null;
            handler = x =>
            {
                received++;
                bus.Unsubscribe(handler);
            };
            bus.Subscribe(handler);
            bus.Subscribe<TestEvent>(x => received++);

            bus.Publish(new TestEvent(1));
            bus.Publish(new TestEvent(1));

            Assert.AreEqual(3, received);
        }

        [Test]
        public void HistoryRecordsEvents()
        {
            EventBus bus = new EventBus();
            bus.Publish(new TestEvent(1));
            Assert.AreEqual(1, bus.History.Count);
            Assert.AreEqual(nameof(TestEvent), bus.History[0].Type);
        }

        private sealed class TestEvent : GameEvent
        {
            public readonly int Amount;

            public TestEvent(int amount) : base("test")
            {
                Amount = amount;
            }
        }
    }
}
