using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using snmpMon.Models;
using snmpMon.Resources.Constrols.Frames;
using snmpMon.Sensors.SensorsServices;
using snmpMon.Services;
using snmpMon.Services.Agent;
using snmpMon.Services.SensorServices;
using snmpMon.Services.WindowServices;
using snmpMon.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace snmpMon.ViewModel
{
    public class DevicesVM : ObservableObject
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
        public ICommand TestCMD {  get; set; }
        private readonly IDeviceWindowService _windowService;
        private readonly IDevicesServices _devicesServices;
        public DevicesVM(IDeviceWindowService windowService, IDevicesServices devicesServices)
        {
            _windowService = windowService;
            _devicesServices = devicesServices;
            _windowService.OnDeviceDetails += _windowService_OnDeviceAdded;
            _ = GetAllDevices();

            AddDev = new RelayCommand(() =>
            {
                _windowService.ShowAddDeviceWindow();

            });

            TestCMD = new RelayCommand<DeviceInfo>(d =>
            {
                _windowService.ShowDeviceMonitoringWin(d);
            });
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
            foreach (var d in _devicesList)
            {
                var agentOrch = App.serviceProvider.GetRequiredService<IMonitoringAgentServices>();

                DeviceInfo deviceInfo = new DeviceInfo()
                {
                    DeviceName = string.Empty,
                    IPAddress = d.IPAddress,
                    CommunityString = d.CommunityString,
                };
                Devices.Add(deviceInfo);

                _= agentOrch.StartMonitoring(deviceInfo.IPAddress, d.CommunityString, deviceInfo);
                

            }
        }



    }
}
