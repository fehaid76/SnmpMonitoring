using snmpMon.Engine.Helper.EventHandle;
using snmpMon.Helper.EventHandle;
using snmpMon.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace snmpMon.Services.SensorServices
{
    public interface IDashboardSensors : IDisposable
    {
        event Action<List<DeviceInterface>> OnDeviceInterfaceReceived;
        event EventHandler<InterfaceStatusEventArgs> OnInterfaceStatusChanged;
        event Action<TimeSpan> OnUpTimeReceived;

        event EventHandler<ResourceUsageEventArgs> OnResourceDeviceUsage;
        event EventHandler<DeviceStatusEventArgs> OnDeviceStatusChanged;
        void StartMonitoring(string ipAddress, string communityString, string deviceCategory);

    }
}