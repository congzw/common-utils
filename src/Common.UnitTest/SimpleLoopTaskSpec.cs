using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable CheckNamespace

namespace Common
{
    [TestClass]
    public class SimpleLoopTaskSpec
    {
        [TestMethod]
        public void Start_Should_LoopBySpan()
        {
            var mockTask = MockTask.Create();
            int loopCount = 3;

            using (var loopTask = new SimpleLoopTask())
            {
                loopTask.LoopSpan = TimeSpan.FromMilliseconds(50);
                loopTask.LoopAction = mockTask.MockAction;
                loopTask.AfterExitLoopAction = mockTask.MockStopAction;

                loopTask.Start();
                Task.Delay(TimeSpan.FromMilliseconds(50 * loopCount + 20)).Wait();

                mockTask.StopInvoked.ShouldFalse();
            }

            (mockTask.InvokeCount >= 3).ShouldTrue();
            (mockTask.InvokeCount <= 4).ShouldTrue();
            mockTask.StopInvoked.ShouldTrue();
        }

        [TestMethod]
        public void Stop_Should_StopAfterLooping()
        {
            var mockTask = MockTask.Create();
            int loopCount = 3;

            //todo: fix failed sometimes
            using (var loopTask = new SimpleLoopTask())
            {
                loopTask.LoopSpan = TimeSpan.FromMilliseconds(50);
                loopTask.LoopAction = mockTask.MockAction;
                loopTask.AfterExitLoopAction = mockTask.MockStopAction;

                loopTask.Start();
                Task.Delay(TimeSpan.FromMilliseconds(50 * loopCount + 20)).Wait();

                loopTask.Stop();
                Task.Delay(TimeSpan.FromMilliseconds(50 * 2)).Wait();

                mockTask.StopInvoked.ShouldTrue();
            }
            (mockTask.InvokeCount >= 4).ShouldTrue();
            (mockTask.InvokeCount <= 5).ShouldTrue();
            mockTask.StopInvoked.ShouldTrue();
        }
    }

    public class MockTask
    {
        public int InvokeCount { get; set; }

        public Action MockAction { get; set; }
        public Action MockStopAction { get; set; }
        public bool StopInvoked { get; set; }

        public static MockTask Create()
        {
            var mockTask = new MockTask();
            mockTask.MockAction = () =>
            {
                mockTask.InvokeCount++;
                AssertHelper.WriteLine("task running at: " + DateTime.Now.ToString("yyyyMMdd HH:mm:ss:fff"));
            };

            mockTask.MockStopAction = () =>
            {
                mockTask.StopInvoked = true;
                AssertHelper.WriteLine("task stopping at: " + DateTime.Now.ToString("yyyyMMdd HH:mm:ss:fff"));
            };
            return mockTask;
        }
    }
}
