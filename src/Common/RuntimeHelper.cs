using System.Runtime.InteropServices;
// ReSharper disable CheckNamespace

namespace Common
{
    public class RuntimeHelper
    {
        public bool IsOsPlatform(params OSPlatform[] osPlatforms)
        {
            foreach (var osPlatform in osPlatforms)
            {
                if (RuntimeInformation.IsOSPlatform(osPlatform))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsProcessArchitecture(params Architecture[] architectures)
        {
            foreach (var architecture in architectures)
            {
                if (RuntimeInformation.ProcessArchitecture == architecture)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsOsArchitecture(params Architecture[] architectures)
        {
            foreach (var architecture in architectures)
            {
                if (RuntimeInformation.OSArchitecture == architecture)
                {
                    return true;
                }
            }
            return false;
        }

        #region for simple use

        public static RuntimeHelper Instance = new RuntimeHelper();

        #endregion
    }
}
