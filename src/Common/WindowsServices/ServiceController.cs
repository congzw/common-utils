using System;
using System.IO;

// ReSharper disable once CheckNamespace
namespace Common.WindowsServices
{
    public interface IServiceController
    {
        WindowServiceInfo ServiceInfo { get; set; }
        MessageResult TryGetStatus();
        MessageResult TryInstall();
        MessageResult TryUninstall();
        MessageResult TryStart();
        MessageResult TryStop();
    }

    public class ServiceController : IServiceController
    {
        public ServiceController(WindowServiceInfo info)
        {
            ServiceInfo = info ?? throw new ArgumentNullException(nameof(info));
        }
        
        public WindowServiceInfo ServiceInfo { get; set; }
        
        public MessageResult TryGetStatus()
        {
            var serviceName = ServiceInfo.ServiceName;
            var serviceState = GetServiceState(serviceName);
            if (serviceState == ServiceState.NotFound)
            {
                return AppendLogsAndResult(false, string.Format("{0} not installed!", serviceName), serviceState.ToString());
            }
            return AppendLogsAndResult(true, string.Format("{0} state: {1}", serviceName, serviceState), serviceState.ToString());
        }

        public MessageResult TryInstall()
        {
            var serviceName = ServiceInfo.ServiceName;
            var serviceFriendlyName = ServiceInfo.ServiceFriendlyName;
            var servicePath = ServiceInfo.ServicePath;

            var exePath = Path.GetFullPath(servicePath);
            if (!File.Exists(exePath))
            {
                return AppendLogsAndResult(false, string.Format("{0} is not found!", exePath));
            }

            var serviceState = GetServiceState(serviceName);
            if (serviceState != ServiceState.NotFound)
            {
                return AppendLogsAndResult(true, string.Format("{0} is already installed!", exePath));
            }

            //this is a bug, todo
            //ServiceInstaller.Install(serviceName, serviceFriendlyName, servicePath);

            AppendLogs("----------");
            AppendLogs(serviceName);
            AppendLogs(serviceFriendlyName);
            AppendLogs(exePath);
            AppendLogs("----------");
            ServiceInstaller.InstallAndStart(serviceName, serviceFriendlyName, exePath);

            GetServiceState(serviceName);
            return AppendLogsAndResult(true, string.Format("{0} install completed!", serviceName));
        }

        public MessageResult TryUninstall()
        {
            var serviceName = ServiceInfo.ServiceName;
            var serviceState = GetServiceState(serviceName);
            if (serviceState == ServiceState.NotFound)
            {
                return AppendLogsAndResult(true, string.Format("{0} not installed!", serviceName));
            }

            ServiceInstaller.Uninstall(serviceName);
            GetServiceState(serviceName);
            return AppendLogsAndResult(true, string.Format("{0} uninstall completed!", serviceName));
        }

        public MessageResult TryStart()
        {
            var serviceName = ServiceInfo.ServiceName;
            var serviceState = GetServiceState(serviceName);
            if (serviceState == ServiceState.NotFound)
            {
                return AppendLogsAndResult(false, string.Format("{0} not installed!", serviceName));
            }

            if (serviceState == ServiceState.Running || serviceState == ServiceState.StartPending)
            {
                return AppendLogsAndResult(true, string.Format("{0} is already running!", serviceName));
            }

            ServiceInstaller.StartService(serviceName);
            return AppendLogsAndResult(true, string.Format("{0} start completed!", serviceName));
        }

        public MessageResult TryStop()
        {
            var serviceName = ServiceInfo.ServiceName;
            var serviceState = GetServiceState(serviceName);
            if (serviceState == ServiceState.NotFound)
            {
                return AppendLogsAndResult(true, string.Format("{0} not installed!", serviceName));
            }

            if (serviceState == ServiceState.Stopped || serviceState == ServiceState.StopPending)
            {
                return AppendLogsAndResult(true, string.Format("{0} is stopping!", serviceName));
            }

            ServiceInstaller.StopService(serviceName);
            GetServiceState(serviceName);
            return AppendLogsAndResult(true, string.Format("{0} stop completed!", serviceName));
        }


        private ServiceState GetServiceState(string serviceName)
        {
            var serviceStatus = ServiceInstaller.GetServiceState(serviceName);
            AppendLogs(string.Format("{0} current state: {1}", serviceName, serviceStatus));
            return serviceStatus;
        }


        private ISimpleLog _log = null;
        private void AppendLogs(string message)
        {
            if (_log == null)
            {
                _log = SimpleLogFactory.Resolve().CreateLogFor(this);
            }
            _log.LogInfo(message);
        }
        private MessageResult AppendLogsAndResult(bool success, string message, object data = null)
        {
            AppendLogs(message);
            return MessageResult.Create(success, message, data);
        }


        #region for easy use

        public static ServiceController Create(WindowServiceInfo info)
        {
            return new ServiceController(info);
        }

        #endregion
    }
    
    public class WindowServiceInfo
    {
        public string ServiceName { get; set; }
        public string ServicePath { get; set; }
        public string ServiceFriendlyName { get; set; }
        
        public static bool Validate(WindowServiceInfo info, out string message)
        {
            if (info == null)
            {
                message = "info should not be null";
                return false;
            }

            if (string.IsNullOrWhiteSpace(info.ServiceName))
            {
                message = "ServiceName should not be null or empty";
                return false;
            }

            if (string.IsNullOrWhiteSpace(info.ServicePath))
            {
                message = "ServicePath should not be null or empty";
                return false;
            }

            message = "OK";
            return true;
        }

        public static WindowServiceInfo Create(string serviceName, string servicePath, string serviceFriendlyName, bool validateFailThrows = false)
        {
            var info = new WindowServiceInfo()
            {
                ServiceName = serviceName,
                ServicePath = servicePath,
                ServiceFriendlyName = serviceFriendlyName
            };

            if (validateFailThrows)
            {
                var success = Validate(info, out var message);
                if (!success)
                {
                    throw new ArgumentException(message);
                }
            }

            return info;
        }
    }
}
