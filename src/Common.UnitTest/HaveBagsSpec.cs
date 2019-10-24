using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common
{
    [TestClass]
    public class HaveBagsSpec
    {
        [TestMethod]
        public void Bags_Implicit_ShouldOk()
        {
            var myBag = new BagsImplicit();
            myBag.SetBagValue("A", 1);
            myBag.GetBagValue("a", 0).ShouldEqual(1);
        }

        [TestMethod]
        public void Bags_Tell_ShouldOk()
        {
            var myBag = new BagsTell();
            myBag.SetBagValue("A", 1);
            myBag.GetBagValue("a", 0).ShouldEqual(1);
        }

        [TestMethod]
        public void Bags_Explicit_ShouldOk()
        {
            var myBag = new BagsExplicit();
            myBag.SetBagValue("A", 1);
            myBag.GetBagValue("a", 0).ShouldEqual(1);
        }

        public class BagsTell : IShouldHaveBags
        {
            public BagsTell()
            {
                Items = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            }

            public IDictionary<string, object> Items { get; set; }

            public string GetBagsPropertyName()
            {
                return nameof(Items);
            }
        }

        public class BagsImplicit : IShouldHaveBags
        {
            public BagsImplicit()
            {
                Bags = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            }
            public IDictionary<string, object> Bags { get; set; }
        }

        public class BagsExplicit : IHaveBags
        {
            public BagsExplicit()
            {
                Bags = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            }
            public IDictionary<string, object> Bags { get; set; }
        }
    }
}
