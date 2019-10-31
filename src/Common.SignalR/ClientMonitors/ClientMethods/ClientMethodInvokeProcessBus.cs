using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.SignalR.ClientMonitors.ClientMethods
{
    //服务器端的处理
    public interface IClientMethodInvokeProcess
    {
        float ProcessOrder { set; get; }
        bool ShouldProcess(ClientMethodInvoke invoke);
        Task ProcessAsync(ClientMethodInvoke invoke);
    }

    public class ClientMethodInvokeProcessBus
    {
        public IEnumerable<IClientMethodInvokeProcess> Processes { get; }

        public ClientMethodInvokeProcessBus(IEnumerable<IClientMethodInvokeProcess> processes)
        {
            Processes = processes;
        }

        public async Task Process(ClientMethodInvoke invoke)
        {
            var sortedProcesses = Processes
                .Where(x => x.ShouldProcess(invoke))
                .OrderBy(x => x.ProcessOrder)
                .ToList();

            foreach (var sortedProcess in sortedProcesses)
            {
                try
                {
                    await sortedProcess.ProcessAsync(invoke).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    //todo log
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}