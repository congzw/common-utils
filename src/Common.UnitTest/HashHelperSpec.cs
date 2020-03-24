using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common
{
    [TestClass]
    public class HashHelperSpec
    {
        [TestMethod]
        public void HashHelper_GetGitHash_ShouldOk()
        {
            //提交一个空文件，Git计算出来的Hash值是e69de29bb2d1d6434b8b29ae775ad8c2e48c5391，实际计算的内容是"blob 0\0"。
            "".GetGitHash().Log().ShouldEqual("e69de29bb2d1d6434b8b29ae775ad8c2e48c5391");
        }

        [TestMethod]
        public void HashHelper_GetHashSha1_ShouldOk()
        {
            "".GetHashSha1().Log();
        }


    }
}
