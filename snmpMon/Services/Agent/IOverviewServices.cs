using snmpMon.Data.Model;
using snmpMon.Helper.EventHandle;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace snmpMon.Services.Agent
{
    public interface IOverviewServices
    {
        event EventHandler<DeviceTrafficEventArgs> DeviceTrafficReceived;
        event Action<double> OnAverageNetworkLatency;
        event Action<int> OnUpDevices;

        Task StartNetworkAverageLatencyCalculateAsync(List<Device> devicesIPs, CancellationTokenSource cancellationToken = null);
        void StartTrafficMonitoring(CancellationTokenSource cancellationToken = null);
        void MemoryCleaner();
    }
}