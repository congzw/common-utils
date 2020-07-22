using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.DI
{
    [TestClass]
    public class MyLifetimeRegistrySpec
    {
        private static IServiceProvider provider;
        [ClassInitialize]
        public static void AutoRegisterTestInitialize(TestContext testContext)
        {
            var services = new ServiceCollection();
            MyLifetimeRegistry.Instance.AutoRegister(services, new[] {
                typeof(IMyLifetime).Assembly,
                typeof(MyLifetimeRegistrySpec).Assembly
            });
            provider = services.BuildServiceProvider();
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void AssembliesNull_Should_Throws()
        {
            var services = new ServiceCollection();
            var myLifetimeRegistry = new MyLifetimeRegistry();
            myLifetimeRegistry.AutoRegister(services, null);
        }


        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void ServicesNull_Should_Throws()
        {
            var myLifetimeRegistry = new MyLifetimeRegistry();
            myLifetimeRegistry.AutoRegister(null, new List<Assembly>());
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
        public void RegisterAfter_Should_Replace()
        {
            var services = new ServiceCollection();

            var myLifetimesHelper = new MyLifetimeRegistry();
            myLifetimesHelper.AutoRegister(services, new[] {
                typeof(IMyLifetime).Assembly,
                typeof(MyLifetimeRegistrySpec).Assembly
            });

            services.AddSingleton<FooTransient>();
            var newProvider = services.BuildServiceProvider();

            using (var scope = newProvider.CreateScope())
            {
                var theFoo = scope.ServiceProvider.GetService<FooTransient>();
                var theFoo2 = scope.ServiceProvider.GetService<FooTransient>();
                Assert.AreSame(theFoo, theFoo2);
            }
        }
    }
}
