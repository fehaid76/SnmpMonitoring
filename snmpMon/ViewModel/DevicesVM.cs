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

namespace snmpMon.ViewModel
{
    public class DevicesVM : ObservableObject , IDisposable
    {
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
            _snmpListener.StartSnmpListener();


            AddDev = new RelayCommand(() =>
            {
                _windowService.ShowAddDeviceWindow();

            });

            MonitoringCMD = new RelayCommand<DeviceInfo>(d =>
            {
                _windowService.ShowDeviceMonitoringWin(d);
                d.ErrorsCount = 0;
                d.WarnningCount = 0;
                d.SuccessCount = 0;
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
            _snmpListener.OnSnmpTrapRecieved += _snmpListener_OnSnmpTrapRecieved;
        }

        private void UnsubscribeEvents()
        {
            _windowService.OnDeviceDetails -= _windowService_OnDeviceAdded;
            _snmpListener.OnSnmpTrapRecieved -= _snmpListener_OnSnmpTrapRecieved;
        }

        private void _snmpListener_OnSnmpTrapRecieved(string arg1, SnmpAlert arg2)
        {
            var device = Devices.FirstOrDefault(d => d.IPAddress ==  arg1);
            if (device != null)
            {
                switch (arg2.AlertLevel)
                {
                    case (int)AlertLevels.Success:
                        device.SuccessCount++;
                        break;
                    case (int)AlertLevels.Error:
                        device.ErrorsCount++;
                        break;
                    case (int)AlertLevels.Warnning:
                        device.WarnningCount++;
                        break;
                }
            }
        }

        private async Task _deleteDevice(DeviceInfo device)
        {
            var isDelete = await _devicesServices.DeleteDeviceAsync(device.IPAddress);
            if (isDelete)
            {
                Devices.Remove(device); 
            }
        }
        private void _windowService_OnDeviceAdded(Device obj)
        {
            if(obj == null) return;
            var agentOrch = App.serviceProvider.GetRequiredService<IMonitoringAgentServices>();

            DeviceInfo deviceInfo = new DeviceInfo()
            {
                DeviceName = string.Empty,
                IPAddress = obj.IPAddress,
                CommunityString = obj.CommunityString,
            };
            Devices.Add(deviceInfo);
            agentOrch.StartMonitoring(obj.IPAddress, obj.CommunityString, deviceInfo);
            _windowService.OnDeviceDetails -= _windowService_OnDeviceAdded;
        }
        private async Task GetAllDevices()
        {

            var _devicesList = await _devicesServices.GetAllDevicesAsync();
            Devices.Clear();
            foreach (var d in _devicesList)
            {
                var agentOrch = App.serviceProvider.GetRequiredService<IMonitoringAgentServices>();

                DeviceInfo deviceInfo = new DeviceInfo()
                {
                    DeviceID = d.ID,
                    DeviceName = d.Name,
                    IPAddress = d.IPAddress,
                    CommunityString = d.CommunityString,
                    Vendor = d.Vendor,      
                };

                Devices.Add(deviceInfo);

                _= agentOrch.StartMonitoring(deviceInfo.IPAddress, d.CommunityString, deviceInfo);
                

            }
        }

        public void Dispose()
        {
            UnsubscribeEvents();
        }
    }
}
