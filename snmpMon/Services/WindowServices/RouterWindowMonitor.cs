using Microsoft.Extensions.DependencyInjection;
using snmpMon.Models;
using snmpMon.Models.WindowAgregation;
using snmpMon.Resources.Constrols.Frames;
using snmpMon.View;
using snmpMon.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Services.WindowServices
{
    public class RouterWindowMonitor : IRouterWindowMonitor
    {
        private readonly Dictionary<string, WindowAggregation<DeviceMonitoringVM, AlertViewModel>> _monitoringWindows;

        public RouterWindowMonitor()
        {
            _monitoringWindows = new Dictionary<string, WindowAggregation<DeviceMonitoringVM, AlertViewModel>>();
        }

        public void OpenMonitoringWindow(DeviceInfo device)
        {
            string ipAddress = device.IPAddress;
            if (!_monitoringWindows.TryGetValue(ipAddress, out var deviceSession))
            {
                deviceSession = new WindowAggregation<DeviceMonitoringVM, AlertViewModel>
                {
                    MainGenericViewModel = App.serviceProvider.GetRequiredService<DeviceMonitoringVM>(),
                    SubGenericViewModel = App.serviceProvider.GetRequiredService<AlertViewModel>()

                };
                _monitoringWindows[ipAddress] = deviceSession;
                

            }

            var window = new DeviceMonitoringWindow();

            window.DeviceMonitoring.DataContext = deviceSession.MainGenericViewModel;

            window.vm.Device = device;
            //if (window.DeviceMonitoring.DataContext is DeviceMonitoringVM devMonitoringVm)
            //{
            //    window.DeviceMonitoring.DataContext = deviceSession.MainGenericViewModel;
            //    devMonitoringVm.Device = device;
            //    window.vm.Device = device;
            //}
            
            
            if (window.DeviceMonitoring is DeviceMonitoring devMon)
            {
                devMon.AlertBox.DataContext = deviceSession.SubGenericViewModel;
            }

            window.Show();
        }
    }
}
