using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using snmpMon.Data.Model;
using snmpMon.Engine.Services.SnmpServices;
using snmpMon.Engine.Services.SnmpTraslatorServices.Model;
using snmpMon.Engine.Services.SnmpTraslatorServices;
using snmpMon.Models;
using snmpMon.Resources.Constrols.Frames;
using snmpMon.Services;
using snmpMon.Services.Agent;
using snmpMon.Services.SensorServices;
using snmpMon.Services.WindowServices;
using snmpMon.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using snmpMon.Helper.Model;
using snmpMon.Engine.Helper;
using snmpMon.Engine.Services.SnmpTraslatorServices.Enums;

namespace snmpMon.ViewModels
{
    public class DevicesVM : ObservableObject , IDisposable
    {
        private bool _isDevicesLoaded = false;
        private List<ActionDisposableAggregation> _MonitoredNodes = new List<ActionDisposableAggregation>();
        private ObservableCollection<DeviceInfo> _devices = new ObservableCollection<DeviceInfo>();

        public ObservableCollection<DeviceInfo> Devices
        {
            get => _devices;
            set
            {
                _devices = value;
                OnPropertyChanged(nameof(Devices));
            }
        }

        public ICommand AddDev {  get; set; }
        public ICommand MonitoringCMD {  get; set; }
        public ICommand DeleteCMD {  get; set; }
        public IAsyncRelayCommand LoadDevicesCommand {  get; set; }
        private readonly IWindowService _windowService;
        private readonly IDevicesServices _devicesServices;
        private readonly ISnmpListener _snmpListener;
        public DevicesVM(IWindowService windowService, IDevicesServices devicesServices, ISnmpListener snmpListener)
        {
            _snmpListener = snmpListener;
            _windowService = windowService;
            _devicesServices = devicesServices;

            subscribeEvents();

            LoadDevicesCommand = new AsyncRelayCommand(GetAllDevices);
            LoadDevicesCommand.Execute(null);


            AddDev = new RelayCommand(() =>
            {
                _windowService.ShowAddDeviceWindow();

            });

            MonitoringCMD = new RelayCommand<DeviceInfo>(d =>
            {
                _windowService.ShowDeviceMonitoringWin(d);

            });

            DeleteCMD = new RelayCommand<DeviceInfo>(async d =>
            {
                if (_windowService.ShowConfirmDialog("Are you sure you want to delete this device?", "Confirm Dialog", MessageBoxIcon.Question))
                {
                    await _deleteDevice(d);

                }
            });
        }

        private void subscribeEvents()
        {


            _windowService.OnDeviceDetails += _windowService_OnDeviceAdded;
        }

        private void UnsubscribeEvents()
        {
            _windowService.OnDeviceDetails -= _windowService_OnDeviceAdded;
        }


        private void StopMonitoringProcess(string ipAddress)
        {
            foreach (var device in _MonitoredNodes)
            {
                if(device.IPAddress == ipAddress)
                {
                    device.actionDisposable.Dispose();
                    return;
                }
            }
            
        }
        private async Task _deleteDevice(DeviceInfo device)
        {
            var isDelete = await _devicesServices.DeleteDeviceAsync(device.IPAddress);
            if (isDelete)
            {
                StopMonitoringProcess(device.IPAddress);
                Devices.Remove(device); 
            }
        }
        private void _windowService_OnDeviceAdded(Device obj)
        {
            if(obj == null) return;
            
            DeviceInfo deviceInfo = new DeviceInfo()
            {
                DeviceName = obj.Name,
                IPAddress = obj.IPAddress,
                CommunityString = obj.CommunityString,
                Vendor = obj.Vendor,
            };
            Devices.Add(deviceInfo);
            _= StartDeviceMonitoringAsync(deviceInfo);



        }


        private async Task GetAllDevices()
        {
            if (_isDevicesLoaded)
                return;
            _isDevicesLoaded = true;
            var _devicesList = await _devicesServices.GetAllDevicesAsync();
            Devices.Clear();
            foreach (var d in _devicesList)
            {

                DeviceInfo deviceInfo = new DeviceInfo()
                {
                    DeviceID = d.ID,
                    DeviceName = d.Name,
                    IPAddress = d.IPAddress,
                    CommunityString = d.CommunityString,
                    Vendor = d.Vendor,      
                    Category = d.Category,
                };

                Devices.Add(deviceInfo);
                _ = StartDeviceMonitoringAsync(deviceInfo);
            }

        }

        private async Task StartDeviceMonitoringAsync(DeviceInfo deviceInfo)
        {
            try
            {

                var agentOrch = App.serviceProvider.GetRequiredService<IMonitoringAgentServices>();
                var monitorTask = await agentOrch.StartMonitoring(deviceInfo.IPAddress, deviceInfo.CommunityString, deviceInfo);

                _MonitoredNodes.Add(monitorTask);

            }
            catch (Exception ex)
            {
                Logger.LogCriticalException(ex, ex.Message);
            }
        }
        public void Dispose()
        {
            UnsubscribeEvents();
        }
    }
}
