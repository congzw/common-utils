using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common
{
    [TestClass]
    public class SimpleQueryStringSpec
    {
        [TestMethod]
        public void CreateQueryString_Null_AutoAppend_Should_Ok()
        {
            var simpleQueryString = new SimpleQueryString();
            var queryString = simpleQueryString.CreateQueryString(null, true);
            queryString.ShouldEqual(string.Empty);
        }

        [TestMethod]
        public void CreateQueryString_Null_NotAutoAppend_Should_Ok()
        {
            var simpleQueryString = new SimpleQueryString();
            var queryString = simpleQueryString.CreateQueryString(null, false);
            queryString.ShouldEqual(string.Empty);
        }

        [TestMethod]
        public void CreateQueryString_EmptyValue_Should_Ok()
        {
            var simpleQueryString = new SimpleQueryString();
            var nvc = new NameValueCollection();
            nvc.Add("a", "1");
            nvc.Add("b", "");
            var queryString = simpleQueryString.CreateQueryString(nvc, false);
            queryString.ShouldEqual("a=1&b=");
        }

        [TestMethod]
        public void CreateQueryString_RepeatKey_Should_Ok()
        {
            var simpleQueryString = new SimpleQueryString();
            var nvc = new NameValueCollection();
            nvc.Add("a", "1");
            nvc.Add("b", "2");
            nvc.Add("b", "3");
            var queryString = simpleQueryString.CreateQueryString(nvc, false);
            queryString.ShouldEqual("a=1&b=2&b=3");
        }

        [TestMethod]
        public void ParseQueryString_Null_Should_Ok()
        {
            var simpleQueryString = new SimpleQueryString();
            var nvc = simpleQueryString.ParseQueryString(null);
            nvc.Count.ShouldEqual(0);
        }

        [TestMethod]
        public void ParseQueryString_EmptyValue_Should_Ok()
        {
            var simpleQueryString = new SimpleQueryString();
            var nvc = simpleQueryString.ParseQueryString("a=1&b=");
            nvc.Count.ShouldEqual(2);
            nvc["A"].ShouldEqual("1");
            nvc["B"].ShouldEqual(string.Empty);
        }

        [TestMethod]
        public void ParseQueryString_RepeatKey_Should_Ok()
        {
            var simpleQueryString = new SimpleQueryString();
            var nvc = simpleQueryString.ParseQueryString("a=1&b=2&b=3");
            nvc.Count.ShouldEqual(2);
            nvc["A"].ShouldEqual("1");
            nvc["B"].ShouldEqual("2,3");
            var values = nvc.GetValues("b");
            values.Length.ShouldEqual(2);
            values[0].ShouldEqual("2");
            values[1].ShouldEqual("3");
        }

        [TestMethod]
        public void ParseQueryStringToDic_Should_Ok()
        {
            var simpleQueryString = new SimpleQueryString();
            var dic = simpleQueryString.ParseQueryStringToDic("a=1&b=2&b=3");
            dic.Count.ShouldEqual(2);
            dic["A"].ShouldEqual("1");
            dic["B"].ShouldEqual("2,3");
        }

        [TestMethod]
        public void CreateQueryStringFromDic_Should_Ok()
        {
            var simpleQueryString = new SimpleQueryString();
            var dic = new Dictionary<string, string>();
            dic.Add("a", "1");
            dic.Add("b", "2");
            var queryString = simpleQueryString.CreateQueryStringFromDic(dic, false);
            queryString.ShouldEqual("a=1&b=2");
        }

        [TestMethod]
        public void CreateQueryStringFromObject_Should_Ok()
        {
            var simpleQueryString = new SimpleQueryString();
            var myObj = new MyObject();
            myObj.A = "1";
            myObj.B = 2;
            var queryString = simpleQueryString.CreateQueryStringFromObject(myObj, false);
            queryString.ShouldEqual("A=1&B=2");
        }

        [TestMethod]
        public void CreateQueryStringFromObject_MultiValues_Should_Ok()
        {
            var simpleQueryString = new SimpleQueryString();
            var myObj = new MyObjectWithMultiValues();
            simpleQueryString.CreateQueryStringFromObject(myObj, false).ShouldEqual("A=1,2&B=2&B=3&C=4&C=5");
            simpleQueryString.CreateQueryStringFromObject(myObj, false, new []{"a","B"}).ShouldEqual("A=1,2&B=2&B=3");
        }

        public class MyObject
        {
            public string A { get; set; }
            public int B { get; set; }
        }

        public class MyObjectWithMultiValues
        {
            public MyObjectWithMultiValues()
            {
                A = "1,2";
                B = new[] { 2, 3 };
                C = new[] { "4", "5" };
            }
            public string A { get; set; }
            public int[] B { get; set; }
            public IList<string> C { get; set; }
        }
    }
}
