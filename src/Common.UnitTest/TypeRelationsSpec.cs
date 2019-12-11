using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common
{
    [TestClass]
    public class TypeRelationsSpec
    {
        [TestMethod]
        public void TryGetBaseType_NotFind_Should_Ok()
        {
            var typeRelations = new TypeRelations();
            typeRelations.TryGetBaseType(typeof(SubC), false).ShouldEqual(null);
            typeRelations.TryGetBaseType(typeof(SubC), true).ShouldEqual(typeof(SubC));
        }

        [TestMethod]
        public void TryGetBaseType_Find_Should_Ok()
        {
            var typeRelations = new TypeRelations();

            typeRelations.Register(typeof(MockBase), typeof(SubA), typeof(SubB));

            typeRelations.TryGetBaseType(typeof(SubC), false).ShouldEqual(null);
            typeRelations.TryGetBaseType(typeof(SubC), true).ShouldEqual(typeof(SubC));
            
            typeRelations.TryGetBaseType(typeof(SubA)).ShouldEqual(typeof(MockBase));
            typeRelations.TryGetBaseType(typeof(SubB)).ShouldEqual(typeof(MockBase));
            typeRelations.TryGetBaseType(typeof(SubC)).ShouldEqual(typeof(SubC));
        }

        public class MockBase
        {

        }

        public class SubA : MockBase
        {

        }
        public class SubB : MockBase
        {

        }
        public class SubC
        {

        }
    }
}
