using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common
{
    [TestClass]
    public class SimpleEventBusSpec
    {
        [TestMethod]
        public void NoRegister_Action_Should_NotInvoked()
        {
            var simpleEventBus = Create();
            var mockEventNotify = new MockMessageNotify(simpleEventBus);
            var mockEventReceiver = new MockEventReceiver();

            mockEventNotify.Send("ABC");
            mockEventReceiver.Invoked.ShouldFalse();
        }

        [TestMethod]
        public void Register_Action_Should_Invoked()
        {
            var simpleEventBus = Create();
            var mockEventNotify = new MockMessageNotify(simpleEventBus);
            var mockEventReceiver = new MockEventReceiver();

            simpleEventBus.Register(@event =>
            {
                mockEventReceiver.Show(@event.Message);
            });

            mockEventNotify.Send("ABC");
            mockEventReceiver.Invoked.ShouldTrue();
        }

        [TestMethod]
        public void Register_Actions_Should_AllInvoked()
        {
            var simpleEventBus = Create();
            var mockEventNotify = new MockMessageNotify(simpleEventBus);
            var mockEventReceiver = new MockEventReceiver();

            simpleEventBus.Register(@event =>
            {
                mockEventReceiver.Show(@event.Message);
            });

            simpleEventBus.Register(@event =>
            {
                mockEventReceiver.Show(@event.Message);
            });

            mockEventNotify.Send("ABC");
            mockEventReceiver.Invoked.ShouldTrue();
            mockEventReceiver.InvokedCount.ShouldEqual(2);
        }
        
        [TestMethod]
        public void Raise_Null_Should_NotInvoked()
        {
            var simpleEventBus = Create();
            var mockEventNotify = new MockMessageNotify(simpleEventBus);
            var mockEventReceiver = new MockEventReceiver();

            simpleEventBus.Register(@event =>
            {
                mockEventReceiver.Show(@event.Message);
            });

            simpleEventBus.ClearActions();

            mockEventNotify.SendNull();
            mockEventReceiver.Invoked.ShouldFalse();
        }

        [TestMethod]
        public void Raise_MultiTime_Should_Invoked()
        {
            var simpleEventBus = Create();
            var mockEventNotify = new MockMessageNotify(simpleEventBus);
            var mockEventReceiver = new MockEventReceiver();

            simpleEventBus.Register(@event =>
            {
                mockEventReceiver.Show(@event.Message);
            });
            
            mockEventNotify.Send("1");
            mockEventNotify.Send("2");
            mockEventNotify.Send("3");
            mockEventReceiver.Invoked.ShouldTrue();
            mockEventReceiver.InvokedCount.ShouldEqual(3);
        }

        [TestMethod]
        public void ClearActions_All_Should_NotInvoked()
        {
            var simpleEventBus = Create();
            var mockEventNotify = new MockMessageNotify(simpleEventBus);
            var mockEventReceiver = new MockEventReceiver();

            simpleEventBus.Register(@event =>
            {
                mockEventReceiver.Show(@event.Message);
            });

            simpleEventBus.ClearActions();

            mockEventNotify.Send("ABC");
            mockEventReceiver.Invoked.ShouldFalse();
        }

        private ISimpleEventBus<AsyncMessageEvent> Create()
        {
            return new SimpleEventBus<AsyncMessageEvent>();
        }
    }

    public class MockMessageNotify
    {
        private readonly ISimpleEventBus<AsyncMessageEvent> _bus;

        public MockMessageNotify(ISimpleEventBus<AsyncMessageEvent> bus)
        {
            _bus = bus;
        }

        public void Send(string message)
        {
            string.Format("Send Is Invoked: {0}", message).Log();
            _bus.Raise(new AsyncMessageEvent(message));
        }
        public void SendNull()
        {
            _bus.Raise(null);
        }
    }

    public class MockEventReceiver
    {
        public int InvokedCount { get; set; }
        public bool Invoked  { get; set; }

        public void Show(string message)
        {
            InvokedCount++;
            Invoked = true;
            string.Format("Show Is Invoked: {0}", message).Log();
        }
    }
}
