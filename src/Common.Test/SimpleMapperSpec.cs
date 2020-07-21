using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common
{
    [TestClass]
    public class SimpleMapperSpec
    {
        [TestMethod]
        public void ShapeData_NotExitProperties_Should_FillNull()
        {
            var instance = Create();
            var mockShapeModel = MockShapeModel.Create();
            dynamic result = instance.ShapeData(mockShapeModel, "Id,Name,Bar", true);
            ((object)result).Log();
            ((int)result.Id).ShouldEqual(1);
            ((string)result.Name).ShouldEqual(mockShapeModel.Name);
            ((object)result.Bar).ShouldNull();
        }

        [TestMethod]
        public void ShapeData_NotExitProperties_Should_FillByArgs()
        {
            var instance = Create();
            var mockShapeModel = MockShapeModel.Create();
            dynamic result = instance.ShapeData(mockShapeModel, "Id,Name,Bar", false);
            ((object)result).Log();
            ((int)result.Id).ShouldEqual(1);
            ((string)result.Name).ShouldEqual(mockShapeModel.Name);
            AssertHelper.ShouldThrows<RuntimeBinderException>(() =>
            {
                var test = result.Bar;
            });
        }

        [TestMethod]
        public void ShapeData_AutoTrim_IgnoreCase_Should_OK()
        {
            var instance = Create();
            var mockShapeModel = MockShapeModel.Create();
            dynamic result = instance.ShapeData(mockShapeModel, " ID, NaMe ", true);
            ((object)result).Log();
            ((int)result.ID).ShouldEqual(1);
            ((string)result.NaMe).ShouldEqual(mockShapeModel.Name);
        }

        [TestMethod]
        public void AsShape_Extensions_Should_OK()
        {
            var mockShapeModel = MockShapeModel.Create();
            dynamic result = mockShapeModel.AsShape(" ID, NaMe ", true);
            ((object)result).Log();
            ((int)result.ID).ShouldEqual(1);
            ((string)result.NaMe).ShouldEqual(mockShapeModel.Name);
        }
        
        private ISimpleMapper Create()
        {
            return new SimpleMapper();
        }
    }

    public class MockShapeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }

        public static MockShapeModel Create(int id = 1)
        {
            return new MockShapeModel()
            {
                Id = id, Name = "Foo" + id, Desc = "FooDesc" + id
            };
        }
    }
}
