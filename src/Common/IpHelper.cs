using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Common
{
    public interface IIpHelper
    {
        IList<string> GetLocalIpAddresses(bool includeVirtualIp);
    }

    public class IpHelper : IIpHelper
    {
        public IList<string> GetLocalIpAddresses(bool includeVirtualIp)
        {
            var localIps = new List<string>();
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    //for VM check: VM IP don't have Default Gateway.
                    var gateway = ni.GetIPProperties().GatewayAddresses.FirstOrDefault();
                    var isVirtualIp = gateway == null;
                    foreach (var ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            if (!isVirtualIp || includeVirtualIp)
                            {
                                localIps.Add(ip.Address.ToString());
                            }
                        }
                    }
                }
            }
            return localIps;
        }
        
        #region for extensions and simple use

        public static IIpHelper Instance => Resolve == null ? _default : Resolve();
        private static readonly IIpHelper _default = new IpHelper();
        public static Func<IIpHelper> Resolve = null;

        #endregion
    }
}
