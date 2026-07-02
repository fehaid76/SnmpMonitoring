using Microsoft.Extensions.DependencyInjection;
using snmpMon.Core.Models;
using snmpMon.Engine.Data;
using snmpMon.Engine.Model;
using snmpMon.Models;
using snmpMon.Resources.Constrols.Frames;
using snmpMon.Services.SensorServices;
using snmpMon.View;
using snmpMon.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace snmpMon.Services.WindowServices
{
    public class DeviceWindowService : IDeviceWindowService
    {
        public event Action<Device> OnDeviceDetails;
        private readonly IRouterWindowMonitor _routerWindowMonitor;
        public DeviceWindowService(IRouterWindowMonitor routerWindowMonitor)
        {
            _routerWindowMonitor = routerWindowMonitor;
        }

        public void ShowAddDeviceWindow()
        {
            var win = App.serviceProvider.GetRequiredService<AddDevicWin>();
            var deviceRepository = App.serviceProvider.GetRequiredService<IDeviceRepository>();
            EventHandler closedHandler = null;

            closedHandler = (s, e) =>
            {
                win.Closed -= closedHandler;

                if (win.DataContext is AddDeviceVM winDataContext && winDataContext != null)
                {
                    OnDeviceDetails?.Invoke(winDataContext.Device);
                    Task.Run(async () =>
                    {
                        DeviceData device = new DeviceData(winDataContext.Device.IPAddress, winDataContext.Device.CommunityString);
                        await deviceRepository.AddDeviceAsync(device);
                    });
                }

                
            };

            win.Closed += closedHandler;

            win.Show();
        }

        public void ShowDeviceMonitoringWin(DeviceInfo device)
        {
            _routerWindowMonitor.OpenMonitoringWindow(device);
            //var win = App.serviceProvider.GetRequiredService<DeviceMonitoringWindow>();
            //win.vm.Device = device;
            //win.Show();
        }

    }
}
