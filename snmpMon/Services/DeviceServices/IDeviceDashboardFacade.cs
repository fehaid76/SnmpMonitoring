using Lextm.SharpSnmpLib;
using snmpMon.Engine.Helper.EventHandle;
using snmpMon.Helper.EventHandle;
using snmpMon.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace snmpMon.Services.DeviceServices
{
    public interface IDeviceDashboardFacade
    {
        event Action<List<DeviceInterface>> OnDeviceInterfaceReceived;
        event EventHandler<DeviceStatusEventArgs> OnDeviceStatusChanged;
        event EventHandler<InterfaceStatusEventArgs> OnInterfaceStatusChanged;
        event EventHandler<ResourceUsageEventArgs> OnResourceDeviceUsage;
        event Action<TimeSpan> OnUpTimeReceived;

        (string MemoryUsage, string cpuTotal1Min) FetchUtilizationNodes(string ip, string community, string category);
        Task GetResourcesAsync(IPEndPoint ip, OctetString community, string memOid, string cpuOid);
        void RegisterEvents();
        Task StartTrackingAsync(string ipAddress, string communityString, string deviceCategory, CancellationToken cancellationToken);
        Task StartDeviceChecker(string ipAddress, string communityString);
        void StopSnmpListener();
    }
}