using snmpMon.Engine.Helper.EventHandle;
using snmpMon.Helper.EventHandle;
using snmpMon.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace snmpMon.Services.DeviceServices
{
    public interface INetworkInterfaceService
    {
        event Action<List<DeviceInterface>> OnDeviceInterfaceReceived;
        event EventHandler<InterfaceStatusEventArgs> InterfaceStatusReceived;
        Task GetInterfacesAsync(string ipAddress, string communityString, CancellationTokenSource cancellationToken = null);
    }
}