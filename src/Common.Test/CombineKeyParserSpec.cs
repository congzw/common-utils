using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common
{
    [TestClass]
    public class CombineKeyParserSpec
    {
        [TestMethod]
        public void CreateCombinedKey_NullOrEmpty_ShouldReturnDefault()
        {
            var parser = CombineKeyParser.NewForTest();
            parser.CreateCombinedKey(null).ShouldEqual(string.Empty);
            parser.CreateCombinedKey(new Dictionary<string, object>()).ShouldEqual(string.Empty);
        }

        [TestMethod]
        public void CreateCombinedKey_AllKey_ShouldInclude()
        {
            var parser = CombineKeyParser.NewForTest();
            var dic = new Dictionary<string, object>();
            dic.Add("A", "a");
            dic.Add("B", "");
            dic.Add("C", "c");
            parser.CreateCombinedKey(dic).ShouldEqual("A=a&B=&C=c");
        }
        
        [TestMethod]
        public void ParseCombinedKey_NullOrEmpty_ShouldReturnDefault()
        {
            var parser = CombineKeyParser.NewForTest();
            parser.ParseCombinedKey(null).Count.ShouldEqual(0);
            parser.ParseCombinedKey("").Count.ShouldEqual(0);
            parser.ParseCombinedKey(" ").Count.ShouldEqual(0);
        }

        [TestMethod]
        public void ParseCombinedKey_AllKey_ShouldInclude()
        {
            var parser = CombineKeyParser.NewForTest();
            var dic = parser.ParseCombinedKey("A=a&B=&c=c");
            dic.Count.ShouldEqual(3);
            dic["A"].ShouldEqual("a");
            dic["B"].ShouldEqual("");
            dic["C"].ShouldEqual("c");
            dic["c"].ShouldEqual("c");
        }
        
        [TestMethod]
        public void AutoGetCombinedKey_ObjectExt_ShouldOk()
        {
            var parser = CombineKeyParser.NewForTest();
            var foo = new CombineKeyFoo();
            parser.AutoGetCombinedKey(foo, "A", "b").ShouldEqual("A=a&b=1");
        }

        [TestMethod]
        public void AutoGetCombinedKey_DicExt_ShouldOk()
        {
            var parser = CombineKeyParser.NewForTest();
            var items = new Dictionary<string, object>();
            items.Add("A", "a");
            items.Add("b", 1);
            items.Add("C", "c");
            parser.AutoGetCombinedKey(items,  "A", "B").ShouldEqual("A=a&B=1");
        }
        
        [TestMethod]
        public void AutoSetCombinedKey_ObjectExt_ShouldOk()
        {
            var parser = CombineKeyParser.NewForTest();
            var foo = new CombineKeyFoo();
            parser.AutoSetCombinedKey(foo, "A", "aa");
            parser.AutoSetCombinedKey(foo, "B", 11);
            foo.A.ShouldEqual("aa");
            foo.B.ShouldEqual(11);
        }

        [TestMethod]
        public void AutoSetCombinedKey_DicExt_ShouldOk()
        {
            var parser = CombineKeyParser.NewForTest();
            var items = new Dictionary<string, object>();
            items.Add("A", "a");
            items.Add("b", 1);
            items.Add("C", "c");

            parser.AutoSetCombinedKey(items, "A", "aa");
            parser.AutoSetCombinedKey(items, "B", 11);
            parser.AutoSetCombinedKey(items, "D", "d");

            items["A"].ShouldEqual("aa");
            items["b"].ShouldEqual(11);
            items["C"].ShouldEqual("c");
        }

        [TestMethod]
        public void SetCombinedKeyByProperties_GenericExt_ShouldOk()
        {
            var parser = CombineKeyParser.NewForTest();
            var foo = new Foo();
            foo.A = "a";
            foo.B = 1;
            foo.SetCombinedKeyByProperties(parser).MyKey.ShouldEqual("A=a&B=1");
        }

        [TestMethod]
        public void SetCombinedKeyByProperties_ObjectExt_ShouldOk()
        {
            var parser = CombineKeyParser.NewForTest();
            var bar = new Bar();
            bar.A = "a";
            bar.B = 1;
            bar.SetCombinedKeyByProperties("MyKey", new []{"A","b"}, parser).MyKey.ShouldEqual("A=a&b=1");
        }

        [TestMethod]
        public void SetPropertiesByCombinedKey_GenericExt_ShouldOk()
        {
            var parser = CombineKeyParser.NewForTest();
            var foo = new Foo();
            foo.MyKey = "A=a&b=1";
            foo.SetPropertiesByCombinedKey(parser);

            foo.A.ShouldEqual("a");
            foo.B.ShouldEqual(1);
        }
        
        [TestMethod]
        public void SetPropertiesByCombinedKey_ObjectExt_ShouldOk()
        {
            var parser = CombineKeyParser.NewForTest();
            var foo = new Bar();
            foo.MyKey = "A=a&b=1";
            foo.SetPropertiesByCombinedKey("MyKey", new[] { "A", "B" }, parser);

            foo.A.ShouldEqual("a");
            foo.B.ShouldEqual(1);
        }

        public class CombineKeyFoo
        {
            public CombineKeyFoo()
            {
                A = "a";
                B = 1;
            }

            public string A { get; set; }
            public int B { get; set; }
            public string X { get; set; }
            public int[] Y { get; set; }
        }

        public class Foo : IHasCombineKey<Foo>
        {
            public string A { get; set; }
            public int B { get; set; }
            public string X { get; set; }
            public int[] Y { get; set; }

            public string MyKey { get; set; }

            public string[] GetIncludePropertyNames()
            {
                return new[] { "A", "B" };
            }

            public string GetCombinedKeyName()
            {
                return "MyKey";
            }
        }

        public class Bar 
        {
            public string A { get; set; }
            public int B { get; set; }
            public string X { get; set; }
            public int[] Y { get; set; }

            public string MyKey { get; set; }

            public string[] GetIncludeKeyNames()
            {
                return new[] { "A", "B" };
            }

            public string GetCombinedKeyName()
            {
                return "MyKey";
            }
        }
    }
}
