using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.DI
{
    [TestClass]
    public class MyLifetimesAutoRegisterSpec
    {
        static IServiceCollection services;
        static IServiceProvider provider;
        [ClassInitialize]
        public static void AutoRegisterTestInitialize(TestContext testContext)
        {
            services = new ServiceCollection();
            MyLifetimesAutoRegister.AutoBind(services, new[] {
                typeof(IMyLifetime).Assembly,
                typeof(MyLifetimesAutoRegisterSpec).Assembly
            });

            provider = services.BuildServiceProvider();
        }

        [TestMethod]
        public void Singleton_Should_ReturnOK()
        {
            var theOne3 = provider.GetService<FooSingleton>();
            using (var scope = provider.CreateScope())
            {
                var theOne = scope.ServiceProvider.GetService<FooSingleton>();
                var theOne2 = scope.ServiceProvider.GetService<FooSingleton>();
                Assert.IsNotNull(theOne);
                Assert.IsNotNull(theOne2);
                Assert.AreSame(theOne, theOne2);
                Assert.AreSame(theOne, theOne3);
            }
        }

        [TestMethod]
        public void MyScope_Should_OK()
        {
            var disposed = false;
            using (var scope = provider.CreateScope())
            {
                var theOne = scope.ServiceProvider.GetService<FooScope>();
                theOne.DisposeInvoked = () => disposed = true;
                var theOne2 = scope.ServiceProvider.GetService<FooScope>();
                Assert.IsNotNull(theOne);
                Assert.IsNotNull(theOne2);
                Assert.AreSame(theOne, theOne2);
            }
            Assert.IsTrue(disposed);
        }
        
        [TestMethod]
        public void Transient_Should_OK()
        {
            using (var scope = provider.CreateScope())
            {
                var theOne = scope.ServiceProvider.GetService<FooTransient>();
                var theOne2 = scope.ServiceProvider.GetService<FooTransient>();
                Assert.IsNotNull(theOne);
                Assert.IsNotNull(theOne2);
                Assert.AreNotSame(theOne, theOne2);
            }
        }
        
        [TestMethod]
        public void Default_Should_OK()
        {
            using (var scope = provider.CreateScope())
            {
                var theOne = scope.ServiceProvider.GetService<FooDefault>();
                var theOne2 = scope.ServiceProvider.GetService<FooDefault>();
                Assert.IsNotNull(theOne);
                Assert.IsNotNull(theOne2);
                Assert.AreNotSame(theOne, theOne2);
            }
        }


        [TestMethod]
        public void NotAutoBind_Should_Ok()
        {
            using (var scope = provider.CreateScope())
            {
                var theFoo = scope.ServiceProvider.GetService<Foo>();
                Assert.IsNull(theFoo);
            }
        }
    }
}
