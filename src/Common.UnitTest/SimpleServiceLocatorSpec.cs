using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common
{
    [TestClass]
    public class SimpleServiceLocatorSpec
    {
        [TestMethod]
        public void GetService_NotRegister_Should_Ok()
        {
            var locator = new SimpleServiceLocator();
            locator.GetService<MockC>().ShouldNull();
            locator.GetServices<IMock>().Count().ShouldEqual(0);
        }
        
        [TestMethod]
        public void GetService_Registered_Should_Ok()
        {
            var locator = Create();
            var services = locator.GetServices<IMock>().ToList();
            services.Log();

            locator.GetService<MockA>().ShouldNotNull();
            locator.GetService<MockB>().ShouldNotNull();
            locator.GetService<MockC>().ShouldNotNull();

            services.Count.ShouldEqual(2);
        }

        [TestMethod]
        public void GetService_Remove_Should_Ok()
        {
            var locator = Create();
            var resolver = (ISimpleServiceResolver)locator;

            resolver.Remove<MockA>();

            var mockB = locator.GetService<MockB>();
            mockB.Log();
            mockB.ShouldNotNull();
            mockB.GetType().ShouldEqual(typeof(MockB));

            var services = locator.GetServices<IMock>().ToList();
            services.Log();
            services.Count.ShouldEqual(1);
        }


        private IServiceLocator Create()
        {
            var resolver = new SimpleServiceLocator();
            resolver.Add<IMock, MockA>(() => new MockA());
            resolver.Add<IMock, MockB>(() => new MockB());
            resolver.Add(() => new MockC());

            //get IMock => A,B
            //get MockA => A
            return resolver;
        }
        
        public interface IMock
        {

        }

        public class MockA : IMock
        {

        }

        public class MockB : IMock
        {

        }
        
        public class MockC
        {

        }
    }

}
