using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common
{

    [TestClass]
    public class CombineKeySpec
    {
        [TestMethod]
        public void SetKeyByProperties_Ignore_ShouldOk()
        {
            var demoCombineModel = new DemoCombineModel();
            demoCombineModel.BarId = "Bar01";
            demoCombineModel.FooId = "Foo01";
            demoCombineModel.BlahId = "Blah01";
            demoCombineModel.IgnorePropertyNames.Add("BlahId");
            demoCombineModel.SetKeyByProperties();
            demoCombineModel.CombineKey.ShouldEqual("FooId=Foo01;BarId=Bar01");
        }

        [TestMethod]
        public void SetKeyByProperties_NoIgnore_ShouldOk()
        {
            var demoCombineModel = new DemoCombineModel();
            demoCombineModel.BarId = "Bar01";
            demoCombineModel.FooId = "Foo01";
            demoCombineModel.BlahId = "Blah01";
            demoCombineModel.SetKeyByProperties();
            demoCombineModel.CombineKey.ShouldEqual("FooId=Foo01;BarId=Bar01;BlahId=Blah01");
        }

        [TestMethod]
        public void SetKeyByProperties_ShouldOk()
        {
            var demoCombineModel = new DemoCombineModel();
            demoCombineModel.CombineKey = "FooId=Foo01;BarId=Bar01;BlahId=Blah01";
            demoCombineModel.IgnorePropertyNames.Add("BlahId");
            demoCombineModel.SetPropertiesByKey();
            demoCombineModel.FooId.ShouldEqual("Foo01");
            demoCombineModel.BarId.ShouldEqual("Bar01");
            demoCombineModel.BlahId.ShouldEqual("Blah01");
        }

        [TestMethod]
        public void SetKeyByProperties_NoProperty_ShouldOk()
        {
            var demoCombineModel = new DemoCombineModel();
            demoCombineModel.CombineKey = "FooId=Foo01;BarId=Bar01;AAA=AAA";
            demoCombineModel.SetPropertiesByKey();
            demoCombineModel.FooId.ShouldEqual("Foo01");
            demoCombineModel.BarId.ShouldEqual("Bar01");
            demoCombineModel.BlahId.ShouldEqual(null);
        }
    }

    public class DemoCombineModel : BaseCombineKey<DemoCombineModel>
    {
        public string FooId { get; set; }
        public string BarId { get; set; }
        public string BlahId { get; set; }

        public static DemoCombineModel Default = new DemoCombineModel();
    }
}
