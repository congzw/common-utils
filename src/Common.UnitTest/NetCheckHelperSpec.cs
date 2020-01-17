using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common
{
    [TestClass]
    public class NetCheckHelperSpec
    {
        [TestMethod]
        public async Task CheckHttpGetAsync_MoreThenTimeout_ShouldFail()
        {
            var netCheckHelper = NetCheckHelper.Instance;
            var checkHttpGetAsync = await netCheckHelper.CheckHttpGetAsync("http://bing.com", TimeSpan.FromMilliseconds(2));
            checkHttpGetAsync.Log();
            checkHttpGetAsync.ShouldFalse();
        }

        [TestMethod]
        public async Task CheckHttpGetAsync_BadUri_ShouldFail()
        {
            var netCheckHelper = NetCheckHelper.Instance;
            var checkHttpGetAsync = await netCheckHelper.CheckHttpGetAsync("http://abc.xyzfoo");
            checkHttpGetAsync.Log();
            checkHttpGetAsync.ShouldFalse();
        }


        [TestMethod]
        public async Task PingAsync_SelfHost_ShouldFail()
        {
            var netCheckHelper = NetCheckHelper.Instance;
            var checkResult = await netCheckHelper.PingAsync("127.0.0.1");
            checkResult.Log();
            checkResult.ShouldTrue();
        }

        [TestMethod]
        public async Task PingAsync_BadHost_ShouldFail()
        {
            var netCheckHelper = NetCheckHelper.Instance;
            var checkResult = await netCheckHelper.PingAsync("2.3.4.5");
            checkResult.Log();
            checkResult.ShouldFalse();
        }
    }
}
