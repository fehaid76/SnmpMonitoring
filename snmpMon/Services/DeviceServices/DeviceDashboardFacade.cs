using Lextm.SharpSnmpLib;
using snmpMon.Core.Services;
using snmpMon.Engine.Helper.EventHandle;
using snmpMon.Engine.Services.SnmpServices;
using snmpMon.Helper.EventHandle;
using snmpMon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace snmpMon.Services.DeviceServices
{
    public class DeviceDashboardFacade : IDeviceDashboardFacade
    {
        public event EventHandler<DeviceStatusEventArgs> OnDeviceStatusChanged;
        public event EventHandler<ResourceUsageEventArgs> OnResourceDeviceUsage;
        public event Action<List<DeviceInterface>> OnDeviceInterfaceReceived;
        public event EventHandler<InterfaceStatusEventArgs> OnInterfaceStatusChanged;
        public event Action<TimeSpan> OnUpTimeReceived;

        private readonly ISnmpListener _snmpListener;
        private readonly IDeviceAvailabilityChecker _deviceAvailability;
        private readonly IDeviceUptimeService _deviceUptimeService;
        private readonly INetworkInterfaceService _networkInterfaceService;
        private readonly IResourceMonitoringService _resourceMonitoringService;

        public DeviceDashboardFacade(ISnmpListener snmpListener, IDeviceAvailabilityChecker deviceAvailability, IDeviceUptimeService deviceUptimeService
            , INetworkInterfaceService networkInterfaceService, IResourceMonitoringService resourceMonitoringService)
        {
            _snmpListener = snmpListener;
            _deviceAvailability = deviceAvailability;
            _deviceUptimeService = deviceUptimeService;
            _networkInterfaceService = networkInterfaceService;
            _resourceMonitoringService = resourceMonitoringService;
            RegisterEvents();

        }

        public void RegisterEvents()
        {
            _deviceAvailability.DevicePowerRestored += (arg1, arg2) =>
            {
                OnDeviceStatusChanged?.Invoke(this, new DeviceStatusEventArgs(arg1, arg2, Enums.DeviceStatus.Up));
            };

            _resourceMonitoringService.OnResourceDeviceUsage += (sender, e) =>
            {
                OnResourceDeviceUsage?.Invoke(this, new ResourceUsageEventArgs(e.MemoryUsage, e.CPUutilization));
            };

            _networkInterfaceService.OnDeviceInterfaceReceived += (arg1) =>
            {
                OnDeviceInterfaceReceived?.Invoke(arg1);
            };

            _snmpListener.OnInterfaceStatusChanged += _snmpListener_OnInterfaceStatusChanged;

            _deviceUptimeService.OnUpTimeChanged += (span) =>
            {
                OnUpTimeReceived?.Invoke(span);
            };
        }

        private void _snmpListener_OnInterfaceStatusChanged(object sender, InterfaceStatusEventArgs e)
        {
            OnInterfaceStatusChanged?.Invoke(this, e);
        }

        public async Task StartTrackingAsync(string ipAddress, string communityString, string deviceCategory, CancellationToken cancellationToken)
        {
            var taskDuration = TimeSpan.FromSeconds(5); 
            var upTimeTask = _deviceUptimeService.StartTrackingAsync(ipAddress, communityString, cancellationToken).WaitAsync(taskDuration);
            var interfacesTask = _networkInterfaceService.GetInterfacesAsync(ipAddress, communityString).WaitAsync(taskDuration);
            await Task.WhenAll(upTimeTask, interfacesTask);
        }

        public async Task GetResourcesAsync(IPEndPoint ip, OctetString community, string memOid, string cpuOid)
        {
            await _resourceMonitoringService.GetResourceUsage(ip, community, memOid, cpuOid);
            
        }

        public (string MemoryUsage, string cpuTotal1Min) FetchUtilizationNodes(string ip, string community, string category)
        {
            var res = _resourceMonitoringService.FetchUtilizationNodes(ip, community, category);
            return (res.MemoryUsage, res.cpuTotal1Min);
        }

        public async Task StartDeviceChecker(string ipAddress, string communityString)
        {
            await _deviceAvailability.StartDeviceChecker(ipAddress, communityString);
        }

        public void StopSnmpListener()
        {
            _snmpListener.StopSnmpListener();
        }
    }
}
