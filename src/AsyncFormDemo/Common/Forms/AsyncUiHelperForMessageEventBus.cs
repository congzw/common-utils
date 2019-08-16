using System;
using System.Windows.Forms;

// ReSharper disable once CheckNamespace
namespace Common
{
    public class AsyncUiHelperForMessageEventBus : AsyncUiHelper, IDisposable
    {
        public ISimpleEventBus<AsyncMessageEvent> Bus { get; set; }

        public AsyncUiHelperForMessageEventBus(Control invokeControl, UpdateUiMessageDelegate updateUiMessageDelegate, ISimpleEventBus<AsyncMessageEvent> bus) : base(invokeControl, updateUiMessageDelegate)
        {
            Bus = bus;
            Bus.Register(UpdateUi);
            invokeControl.Disposed += InvokeControl_Disposed;
        }

        private void InvokeControl_Disposed(object sender, EventArgs e)
        {
            CloseUi();
        }

        private void UpdateUi(AsyncMessageEvent messageEvent)
        {
            SafeUpdateUi(messageEvent.Message, messageEvent.DateTimeEventOccurred);
        }

        private bool _closed = false;
        public void CloseUi()
        {
            Bus.RemoveRegister(UpdateUi);
            _closed = true;
        }

        public void Dispose()
        {
            if (_closed)
            {
                return;
            }
            CloseUi();
        }
    }

    public static class AsyncUiHelperForMessageEventBusExtensions
    {
        public static AsyncUiHelperForMessageEventBus CreateAsyncUiHelperForMessageEventBus(this Control invokeControl, UpdateUiMessageDelegate updateMessage, ISimpleEventBus<AsyncMessageEvent> bus = null)
        {
            return new AsyncUiHelperForMessageEventBus(invokeControl, updateMessage, bus ?? SimpleEventBus<AsyncMessageEvent>.Resolve());
        }
    }
}
