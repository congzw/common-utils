using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common
{
    [TestClass]
    public class SimpleLazyFactorySpec
    {
        [TestMethod]
        public void Default_Multi_Call_Should_Ex()
        {
            AssertHelper.ShouldThrows<Exception>(() =>
            {
                var defaultCount = new InstanceCounter();
                var simpleLazyFactory = SimpleLazyFactory<IFooService>.CreateForTest();
                simpleLazyFactory.Default(() => new FooService(defaultCount));
                simpleLazyFactory.Default(() => new FooService(defaultCount));
            });
        }

        [TestMethod]
        public void Default_Not_Init_Should_Ex()
        {
            AssertHelper.ShouldThrows<Exception>(() =>
            {
                var simpleLazyFactory = SimpleLazyFactory<IFooService>.CreateForTest();
                simpleLazyFactory.Resolve();
            });
        }

        [TestMethod]
        public void Reset_Multi_Time_Should_Ok()
        {
            var counter1 = new InstanceCounter();
            var counter2 = new InstanceCounter();
            var simpleLazyFactory = SimpleLazyFactory<IFooService>.CreateForTest();
            simpleLazyFactory.Reset(() => new FooService(counter1));
            simpleLazyFactory.Reset(() => new FooService(counter2));
            var resolve = simpleLazyFactory.Resolve;
            for (int i = 0; i < 3; i++)
            {
                var service = resolve();
            }
            counter1.TotalCount.ShouldEqual(0);
            counter2.TotalCount.ShouldEqual(1);
        }

        [TestMethod]
        public void Default_Multi_Call_Should_Return_Same()
        {
            var defaultCount = new InstanceCounter();
            var simpleLazyFactory = SimpleLazyFactory<IFooService>.CreateForTest();
            var defaultFunc = simpleLazyFactory.Default(() => new FooService(defaultCount)).Resolve;
            var fooService = defaultFunc();
            var fooService2 = defaultFunc();
            defaultCount.TotalCount.ShouldEqual(1);
            fooService.ShouldEqual(fooService2);
        }

        [TestMethod]
        public void Default_Multi_Call_Should_Singleton()
        {
            var defaultCount = new InstanceCounter();
            var simpleLazyFactory = SimpleLazyFactory<IFooService>.CreateForTest();
            var defaultFunc = simpleLazyFactory.Default(() => new FooService(defaultCount)).Resolve;
            for (int i = 0; i < 10; i++)
            {
                defaultFunc();
            }
            defaultCount.TotalCount.ShouldEqual(1);
        }

        [TestMethod]
        public void Reset_Should_Replace_Default()
        {
            var defaultCount = new InstanceCounter();
            var simpleLazyFactory = SimpleLazyFactory<IFooService>.CreateForTest();
            var defaultFunc = simpleLazyFactory.Default(() => new FooService(defaultCount)).Resolve;

            var instanceCount = new InstanceCounter();
            var theOne = new FooService(instanceCount);
            var resetFunc = simpleLazyFactory.Reset(() => theOne).Resolve;

            var fooService = defaultFunc();
            var fooService2 = resetFunc();
            fooService.ShouldEqual(fooService2);

            for (int i = 0; i < 10; i++)
            {
                defaultFunc();
            }

            for (int i = 0; i < 10; i++)
            {
                resetFunc();
            }

            defaultCount.TotalCount.ShouldEqual(0);
            instanceCount.TotalCount.ShouldEqual(1);
        }

        [TestMethod]
        public void Static_Resolve_Default_Should_Call_Once()
        {
            for (int i = 0; i < 3; i++)
            {
                var fooService = FooService.Resolve();
                fooService.Counter.TotalCount.ShouldEqual(1);
            }
            
            FooService.Resolve.ShouldEqual(SimpleLazyFactory<IFooService>.Instance.Resolve);
        }

        public interface IFooService
        {
            InstanceCounter Counter { get; }
        }

        public class FooService : IFooService
        {
            private static readonly object _lock = new object();

            public FooService(InstanceCounter instanceCount)
            {
                Counter = instanceCount;
                lock (_lock)
                {
                    instanceCount.TotalCount++;
                    this.LogHashCode();
                }
            }

            public InstanceCounter Counter { get; }


            #region for di extensions

            public static Func<IFooService> Resolve { get; }
                = SimpleLazyFactory<IFooService>.Instance.Default(() => new FooService(new InstanceCounter())).Resolve;

            #endregion
        }

        public class InstanceCounter
        {
            public int TotalCount { get; set; }
        }
    }
}
