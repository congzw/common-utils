using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common
{
    [TestClass]
    public class SimpleConstFieldSpec
    {
        [TestMethod]
        public void GetDescriptionItems_Class_ShouldFind()
        {
            var constFieldValues = DescriptionItemHelper.Instance.GetDescriptionItems(typeof(MockConst));
            Show(constFieldValues);
            constFieldValues.Count.ShouldEqual(7);
        }

        [TestMethod]
        public void GetDescriptionItems_Enum_ShouldFind()
        {
            var constFieldValues = DescriptionItemHelper.Instance.GetDescriptionItems(typeof(MockEnum));
            Show(constFieldValues);
            constFieldValues.Count.ShouldEqual(2);
        }

        [TestMethod]
        public void GetDescriptionItems_Assembly_ShouldFind()
        {
            var constFieldValues = typeof(MockEnum).Assembly.GetDescriptionItems();
            Show(constFieldValues);
            constFieldValues.Count.ShouldEqual(7 + 2);
        }

        [TestMethod]
        public void ExportDescriptionContents_Assembly_ShouldFind()
        {
            var constFieldValues = typeof(MockEnum).Assembly.ExportDescriptionContents();
            constFieldValues.Log();

            Func<Type, DescriptionItemValue, string> formatter = (t, x) => string.Format("{0}, {1}, {2}, [{3}]", t?.FullName,
                x.FieldName, x.FieldValue, x.Description);
            var constFieldValues2 = typeof(MockEnum).Assembly.ExportDescriptionContents(formatter);
            constFieldValues2.Log();
        }

        private void Show(IEnumerable<DescriptionItemValue> list)
        {
            foreach (var constField in list)
            {
                string.Format("{0}, {1}, {2}", constField.FieldName, constField.FieldValue, constField.Description)
                    .Log();
            }
        }

        public class MockConst
        {
            [DescriptionItem(description: "PublicA")]
            public string A { get; set; } = "AAA";

            [DescriptionItem(description: "PublicStaticB")]
            public static string B { get; set; } = "BBB";

            [DescriptionItem(description: "PrivateFieldC")]
            private string C = "PrivateC";

            [DescriptionItem(description: "PrivateStaticFieldD")]
            private static string D = "PrivateStaticD";

            [DescriptionItem(description: "PublicStaticFieldE")]
            private static string E = "PrivateStaticE";

            [DescriptionItem(description: "PublicPropertyX")]
            public string X { get; set; } = "XXX";

            [DescriptionItem(description: "PublicPropertyY", Name = "Name_Y")]
            public string Y { get; set; } = "YYY";
        }

        public enum MockEnum
        {
            [DescriptionItem(description: "Desc_Enum_E1")]
            E1 = 0,
            [DescriptionItem(description: "Desc_Enum_E2", Name = "Name_E2")]
            E2 = 1
        }
    }
}
