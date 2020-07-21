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

        [TestInitialize]
        public void MethodInitialize()
        {
            provider = services.BuildServiceProvider();
        }

        [TestMethod]
        public void Singleton_Should_ReturnOK()
        {
            var theOneWithInterface = provider.GetService<IFooSingleton>();
            using (var scope = provider.CreateScope())
            {
                var theOne = scope.ServiceProvider.GetService<IFooSingleton>();
                var theOne2 = scope.ServiceProvider.GetService<FooSingleton>();
                Assert.IsNotNull(theOne);
                Assert.IsNotNull(theOne2);
                Assert.AreSame(theOne, theOne2);
                Assert.AreSame(theOne, theOneWithInterface);
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

        [TestMethod]
        public void Ignore_Should_Ok()
        {
            using (var scope = provider.CreateScope())
            {
                var theFoo = scope.ServiceProvider.GetService<FooIgnore>();
                Assert.IsNull(theFoo);
            }
        }

        [TestMethod]
        public void AfterReplace_Should_Ok()
        {
            services = new ServiceCollection();
            
            MyLifetimesAutoRegister.AutoBind(services, new[] {
                typeof(IMyLifetime).Assembly,
                typeof(MyLifetimesAutoRegisterSpec).Assembly
            });

            services.AddSingleton<FooTransient>();

            provider = services.BuildServiceProvider();

            using (var scope = provider.CreateScope())
            {
                var theFoo = scope.ServiceProvider.GetService<FooTransient>();
                var theFoo2 = scope.ServiceProvider.GetService<FooTransient>();
                Assert.AreSame(theFoo, theFoo2);
            }
        }

        [TestMethod]
        public void BeforeRegistered_Should_Ok()
        {
            services = new ServiceCollection();

            services.AddSingleton<FooTransient>();

            MyLifetimesAutoRegister.AutoBind(services, new[] {
                typeof(IMyLifetime).Assembly,
                typeof(MyLifetimesAutoRegisterSpec).Assembly
            });
            
            provider = services.BuildServiceProvider();

            using (var scope = provider.CreateScope())
            {
                var theFoo = scope.ServiceProvider.GetService<FooTransient>();
                var theFoo2 = scope.ServiceProvider.GetService<FooTransient>();
                Assert.AreSame(theFoo, theFoo2);
            }
        }
    }
}
