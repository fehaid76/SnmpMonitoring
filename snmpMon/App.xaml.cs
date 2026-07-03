using Microsoft.Extensions.DependencyInjection;
using snmpMon.Engine.Services.OidServices;
using snmpMon.Engine.Services.SnmpServices;
using snmpMon.Engine.Services.SnmpTraslatorServices;
using snmpMon.MibJson.Services;
using snmpMon.Resources.Constrols.Frames.FrameServices;
using snmpMon.Services;
using snmpMon.Services.Agent;
using snmpMon.Services.AlertServices;
using snmpMon.Services.SensorServices;
using snmpMon.Services.WindowServices;
using snmpMon.View;
using snmpMon.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using snmpMon.Data;
using snmpMon.Data.Repositories;

namespace snmpMon
{

    public partial class App : Application
    {
        public static IServiceProvider serviceProvider {  get; private set; }

        public App()
        {
            
            var serviceCollection = new ServiceCollection();
            ConfigurationServices(serviceCollection);
            serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigurationServices(IServiceCollection service)
        {
            service.AddTransient<MainViewModel>();
            service.AddTransient<IDevicesServices, DevicesServices>();
            service.AddTransient<CardsViewModel>();
            service.AddTransient<AddDeviceVM>();
            service.AddTransient<AddDevicWin>();
            service.AddTransient<DevicesVM>();
            service.AddTransient<IDeviceWindowService, DeviceWindowService>();
            service.AddTransient<IDeviceRepository, DeviceRepository>();
            service.AddTransient<DeviceMonitoringWindow>();
            service.AddTransient<DeviceMonitoringVM>();
            service.AddTransient<IDashboardSensors, DashboardSensors>();
            // Test
            service.AddTransient<IMonitoringAgentServices,  MonitoringAgentServices>();
            service.AddSingleton<IFrameRequestServices,  FrameRequestServices>();
            service.AddTransient<IAlertNotify, AlertNotify>();
            service.AddTransient<AlertViewModel>();
            service.AddSingleton<IRouterWindowMonitor, RouterWindowMonitor>();

            service.AddSingleton<IMibParser ,MibParser>();

            // Sensors Servies 
            service.AddSingleton<ISnmpListener, SnmpListener>();
            service.AddTransient<ISensor, PingSensor>();
            service.AddTransient<ISensor<string, bool>, PingSensor>();
            
            service.AddTransient<ISensor, DeviceNameSensor>();
            service.AddTransient<ISensor<string, string>, DeviceNameSensor>();

            service.AddTransient<ISensor, PortsCountSensor>();
            service.AddTransient<ISensor<string, int>, PortsCountSensor>();

            // Data Layer 
            service.AddTransient<ISnmpService, SnmpService>();
            service.AddTransient<IOidService, OidService>();
            service.AddTransient<ISnmpTranslator, SnmpTranslator>();
        }


        protected override void OnExit(ExitEventArgs e)
        {
            Environment.Exit(0);
            base.OnExit(e);
        }
    }
}
