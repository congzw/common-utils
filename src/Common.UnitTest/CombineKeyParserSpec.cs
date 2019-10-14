using System;
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
        public void ParseCombinedKey__NullOrEmpty_ShouldReturnDefault()
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
    }
}
