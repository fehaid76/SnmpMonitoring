using Microsoft.Extensions.DependencyInjection;
using snmpMon.Core.Models;
using snmpMon.Data.Model;
using snmpMon.Engine.Model;
using snmpMon.Models;
using snmpMon.Resources.Constrols.Frames;
using snmpMon.Services.SensorServices;
using snmpMon.View;
using snmpMon.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace snmpMon.Services.WindowServices
{
    public class DeviceWindowService : IWindowService
    {
        public event Action<Device> OnDeviceDetails;
        private readonly IRouterWindowMonitor _routerWindowMonitor;
        public DeviceWindowService(IRouterWindowMonitor routerWindowMonitor)
        {
            _routerWindowMonitor = routerWindowMonitor;
        }

        public void ShowInterfaceOverview(string ipAddress, string CommunityString, string ifIndex, string ifDesc)
        {

            var vm = App.serviceProvider.GetRequiredService<InterfaceOverviewViewModel>();
            var win = App.serviceProvider.GetRequiredService<InterfaceOverview>();

            vm.Initialize(ipAddress, CommunityString, ifIndex, ifDesc);

            win.InterfaceOverviewFrm.DataContext = vm;
            win.Show();
        }


        public void ShowAddDeviceWindow()
        {
            var win = App.serviceProvider.GetRequiredService<AddDevicWin>();
            var devicesServices = App.serviceProvider.GetRequiredService<IDevicesServices>();
            EventHandler closedHandler = null;

            closedHandler = (s, e) =>
            {
                win.Closed -= closedHandler;

                if (win.DataContext is AddDeviceVM winDataContext && winDataContext != null)
                {
                    OnDeviceDetails?.Invoke(winDataContext.Device);
                    Task.Run(async () =>
                    {
                        Device device = new Device(winDataContext.Device?.IPAddress, winDataContext.Device?.CommunityString, winDataContext.Device?.Vendor, 
                            winDataContext?.Device.Name, winDataContext.Device.CategoryID);
                        await devicesServices.AddDeviceAsync(device);
                    });
                }

                
            };

            win.Closed += closedHandler;

            win.Show();
        }

        public void ShowDeviceMonitoringWin(DeviceInfo device)
        {
            _routerWindowMonitor.OpenMonitoringWindow(device);

        }
        public bool ShowConfirmDialog(string message, string title, MessageBoxIcon icon)
        {
            var diWin = new ConfirmDialog();
            var dialogDataContext = App.serviceProvider.GetRequiredService<ConfirmDialogVM>();
            diWin.DataContext = dialogDataContext;
            dialogDataContext.Message = message;
            dialogDataContext.Icon = icon;  
            dialogDataContext.Title = title;
            diWin.ShowDialog();
            return dialogDataContext.IsOk;
        }

        
    }
}
