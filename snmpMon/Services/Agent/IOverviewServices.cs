using snmpMon.Data.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace snmpMon.Services.Agent
{
    public interface IOverviewServices
    {
        event Action<double> OnAverageNetworkLatency;
        event Action<double> OnInboundKbps;
        event Action<double> OnOutboundKbps;
        event Action<int> OnUpDevices;
        event Action<string> OnTopInboundDevice;

        Task StartNetworkAverageLatencyCalculateAsync(List<Device> devicesIPs);
        void StartTrafficMonitoring();
        void MemoryCleaner();
    }
}