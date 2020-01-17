using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common
{
    [TestClass]
    public class IpHelperSpec
    {
        [TestMethod]
        public void GetLocalIpAddresses_All_ShouldOk()
        {
            var ipHelper = IpHelper.Instance;
            var localIpAddresses = ipHelper.GetLocalIpAddresses(true);
            localIpAddresses.Log();
        }

        [TestMethod]
        public void GetLocalIpAddresses_NoVirtual_ShouldOk()
        {
            var ipHelper = IpHelper.Instance;
            var localIpAddresses = ipHelper.GetLocalIpAddresses(false);
            localIpAddresses.Log();
        }
    }
}
