using System;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable CheckNamespace
namespace Common
{
    public static class TaskEx
    {
        #region hack for FromResult

        public static Task<T> AsTask<T>(this T value)
        {
            return FromResult(value);
        }

        public static Task<T> FromResult<T>(T result)
        {
#if NET40
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(result);
            return tcs.Task;
#else
            return Task.FromResult(result);
#endif
        }

        #endregion

        #region hack for deplay

        public static Task AsDelay(this TimeSpan value)
        {
            return Delay(value);
        }

        public static Task Delay(TimeSpan span)
        {
#if NET40
            return Delay(span, CancellationToken.None);
#else
            return Task.Delay(span);
#endif
        }

        public static Task Delay(TimeSpan span, CancellationToken token)
        {
#if NET40
            return Delay((int)span.TotalMilliseconds, token);
#else
            return Task.Delay(span, token);
#endif
        }

        private static Task Delay(int milliseconds, CancellationToken token)
        {
            var tcs = new TaskCompletionSource<object>();
            token.Register(() => tcs.TrySetCanceled());
            var timer = new Timer(_ => tcs.TrySetResult(null));
            timer.Change(milliseconds, -1);
            return tcs.Task;
        }

        #endregion
    }
}
