using snmpMon.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace snmpMon.Services.SensorServices
{
    public interface IDashboardSensors : IDisposable
    {
        event Action<double> OnMemoryUsageReceived;
        event Action<int> OnCPUutilizationReceived;
        event Action<List<DeviceInterface>> OnDeviceInterfaceReceived;
        event Action<string, int> OnInterfaceStatusChanged;
        event Action<SystemInformation> OnSystemInformationReceived;
        event Action<TimeSpan> OnUpTimeReceived;
        void StartMonitoring(string ipAddress, string communityString);
        Task GetUpTime();
    }
}