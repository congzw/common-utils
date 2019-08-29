using System;
using System.Windows.Forms;

// ReSharper disable once CheckNamespace
namespace Common
{
    public delegate void UpdateUiMessageDelegate(string value);

    public class AsyncUiHelper
    {
        public AsyncUiHelper(Control invokeControl, UpdateUiMessageDelegate updateUiMessageDelegate)
        {
            InvokeControl = invokeControl;
            UpdateUiMessage = updateUiMessageDelegate;
            WithDatePrefix = true;
            AutoAppendLine = true;
        }
        
        public bool WithDatePrefix { get; set; }
        public bool AutoAppendLine { get; set; }
        public Control InvokeControl { get; set; }
        protected UpdateUiMessageDelegate UpdateUiMessage { get; set; }
        
        //此方法支持在自动判断是否在非创建线程中被调用
        private void UpdateUi(string message, DateTime messageAt)
        {
            string value = string.Format("{0}{1}", message, AutoAppendLine ? Environment.NewLine : string.Empty);
            if (WithDatePrefix)
            {
                value = messageAt.ToString("yyyy-MM-dd HH:mm:ss:fff") + " => " + value;
            }
            if (InvokeControl.InvokeRequired)
            {
                //https://www.cnblogs.com/Rustle/articles/11301.html
                //https://www.cnblogs.com/worldreason/archive/2008/06/09/1216127.html
                //InvokeControl.Invoke(UpdateUiMessage, value);
                InvokeControl.BeginInvoke(UpdateUiMessage, value);
            }
            else
            {
                UpdateUiMessage(value);
            }
        }
        
        public void SafeUpdateUi(string message, DateTime? now = null)
        {
            UpdateUi(message, now ?? DateTime.Now);
        }

        #region for simple use

        public static AsyncUiHelper Create(Control invokeControl, UpdateUiMessageDelegate updateMessage)
        {
            return new AsyncUiHelper(invokeControl, updateMessage);
        }

        #endregion
    }

    public static class AsyncUiHelperExtensions
    {
        public static AsyncUiHelper CreateAsyncUiHelper(this Control invokeControl, UpdateUiMessageDelegate updateMessage)
        {
            return AsyncUiHelper.Create(invokeControl, updateMessage);
        }
    }
}
