using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common
{
    [TestClass]
    public class SimpleConstFieldSpec
    {
        [TestMethod]
        public void ExportConstFields_Class_ShouldFind()
        {
            var constFieldValues = SimpleConstFieldHelper.GetConstFields(typeof(MockConst));
            Show(constFieldValues);
            constFieldValues.Count.ShouldEqual(4);
        }

        [TestMethod]
        public void ExportConstFields_Enum_ShouldFind()
        {
            var constFieldValues = SimpleConstFieldHelper.GetConstFields(typeof(MockEnum));
            Show(constFieldValues);
            constFieldValues.Count.ShouldEqual(2);
        }

        [TestMethod]
        public void ExportConstFields_Assembly_ShouldFind()
        {
            var constFieldValues = typeof(MockEnum).Assembly.ExportConstFields();
            Show(constFieldValues);
            constFieldValues.Count.ShouldEqual(6);
        }

        [TestMethod]
        public void ExportConstFieldContents_Assembly_ShouldFind()
        {
            var constFieldValues = typeof(MockEnum).Assembly.ExportConstFieldContents();
            constFieldValues.Log();

            Func<Type, SimpleConstFieldValue, string> formatter = (t, x) => string.Format("{0}, {1}, {2}, [{3}]", t?.FullName,
                x.FieldName, x.FieldValue, x.Description);
            var constFieldValues2 = typeof(MockEnum).Assembly.ExportConstFieldContents(formatter);
            constFieldValues2.Log();
        }

        private void Show(IEnumerable<SimpleConstFieldValue> list)
        {
            foreach (var constField in list)
            {
                string.Format("{0}, {1}, {2}", constField.FieldName, constField.FieldValue, constField.Description)
                    .Log();
            }
        }

        public class MockConst
        {
            [SimpleConstField(description: "PublicA")]
            public string A { get; set; } = "AAA";

            [SimpleConstField(description: "PublicStaticB")]
            public static string B { get; set; } = "BBB";

            [SimpleConstField(description: "PrivateFieldC")]
            private string C = "PrivateC";

            [SimpleConstField(description: "PrivateStaticFieldC")]
            private static string D = "PrivateStaticD";
            
            [SimpleConstField(description: "PublicPropertyX")]
            public string X { get; set; } = "XXX";

            [SimpleConstField(description: "PublicPropertyY", Name = "Name_Y")]
            public string Y{ get; set; } = "YYY";
        }

        public enum MockEnum
        {
            [SimpleConstField(description: "Desc_Enum_E1")]
            E1 = 0,
            [SimpleConstField(description: "Desc_Enum_E2", Name = "Name_E2")]
            E2 = 1
        }
    }
}
