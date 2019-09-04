using Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable CheckNamespace

namespace Common
{
    [TestClass]
    public class IncludePropertiesSpec
    {
        [TestMethod]
        public void SetProperties_Null_ShouldOk()
        {
            var includeProperties = IncludeProperties.Create().SetProperties(true, "A", "b", "", null);
            includeProperties.Properties.Log().ShouldEqual("A,b");
        }

        [TestMethod]
        public void Properties_Split_ShouldOk()
        {
            var includeProperties = IncludeProperties.Create(true, '|').SetProperties(true, "A", "b");
            includeProperties.Properties.Log().ShouldEqual("A|b");

            var includeProperties2 = IncludeProperties.Create(true).SetProperties(true, "A", "b");
            includeProperties2.Properties.Log().ShouldEqual("A,b");
        }
        
        [TestMethod]
        public void SetProperties_IgnoreCase_ShouldOk()
        {
            var includeProperties = IncludeProperties.Create(true).SetProperties(true, "A", "b");
            includeProperties.Properties.Log();
            includeProperties.HasProperty("A").ShouldTrue();
            includeProperties.HasProperty("B").ShouldTrue();
            includeProperties.HasProperty("C").ShouldFalse();
        }

        [TestMethod]
        public void SetProperties_NotIgnoreCase_ShouldOk()
        {
            var includeProperties = IncludeProperties.Create(false).SetProperties(true, "A", "b");
            includeProperties.Properties.Log();
            includeProperties.HasProperty("A").ShouldTrue();
            includeProperties.HasProperty("B").ShouldFalse();
            includeProperties.HasProperty("C").ShouldFalse();
        }
    }
}
