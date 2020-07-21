using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common
{
    [TestClass]
    public class MyHashHelperSpec
    {
        [TestMethod]
        public void HashHelper_GetHashAsGitBlob_ShouldOk()
        {
            //提交一个空文件，Git计算出来的Hash值是e69de29bb2d1d6434b8b29ae775ad8c2e48c5391，实际计算的内容是"blob 0\0"。
            "".GetHashAsGitBlob().Log().ShouldEqual("e69de29bb2d1d6434b8b29ae775ad8c2e48c5391");
        }

        [TestMethod]
        public void HashHelper_GetHashSha1_ShouldOk()
        {
            "".GetHashMd5().Log();
            "".GetHashSha1().Log();
            "".GetHashSha256().Log();
            "".GetHashSha384().Log();
            "".GetHashSha512().Log();
        }

        [TestMethod]
        public void GuidHashMap_TryFindByHash_ShouldOk()
        {
            var guidHashMap = new GuidHashMap();
            var now = new DateTime(2000, 1, 1);
            for (int i = 0; i < 10; i++)
            {
                guidHashMap.Add(Guid.NewGuid(), now.AddSeconds(1));
            }

            var hashKeys = guidHashMap.Items.Keys.ToList();
            foreach (var hashKey in hashKeys)
            {
                var display = guidHashMap.TryFindByHash(hashKey).Hash;
                display.Log();
            }
        }

        [TestMethod]
        public void GuidHashMap_TryFindByShortHash_ShouldOk()
        {
            var guidHashMap = new GuidHashMap();
            var now = new DateTime(2000, 1, 1);
            for (int i = 0; i < 10; i++)
            {
                guidHashMap.Add(Guid.NewGuid(), now.AddSeconds(1));
            }

            var hashKeys = guidHashMap.Items.Keys.ToList();
            foreach (var hashKey in hashKeys)
            {
                var shortKey = hashKey.Substring(0, 4);
                var display = shortKey + " : " + guidHashMap.TryFindByShortHash(shortKey).Hash;
                display.Log();
            }
        }

        [TestMethod]
        public void GuidHashMap_TryFindAllByShortHash_ShouldOk()
        {
            var guidHashMap = new GuidHashMap();
            var now = new DateTime(2000, 1, 1);
            for (int i = 0; i < 1000; i++)
            {
                guidHashMap.Add(Guid.NewGuid(), now.AddSeconds(1));
            }

            var hashKeys = guidHashMap.Items.Keys.ToList();
            var countGroup = new Dictionary<int, int>();

            foreach (var hashKey in hashKeys)
            {
                var shortKey = hashKey.Substring(0, 4);
                var length = guidHashMap.TryFindAllByShortHash(shortKey).Length;
                if (length > 1)
                {
                    //var display = shortKey + " Find Count: " + length;
                    //display.Log();
                    if (!countGroup.ContainsKey(length))
                    {
                        countGroup[length] = 1;
                    }
                    else
                    {
                        countGroup[length] = countGroup[length] + 1;
                    }
                }
            }

            foreach (var i in countGroup)
            {
                string.Format("find {0} total: {1}", i.Key, i.Value).Log();
            }
        }
    }
}
