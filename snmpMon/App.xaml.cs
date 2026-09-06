using Microsoft.Extensions.DependencyInjection;
using snmpMon.Core.Services;
using snmpMon.Data;
using snmpMon.Data.Repositories;
using snmpMon.Engine.Services.DI;
using snmpMon.Engine.Services.SnmpServices;
using snmpMon.Engine.Services.SnmpTraslatorServices;
using snmpMon.MibJson.Services;
using snmpMon.Resources.Constrols.Frames;
using snmpMon.Resources.Constrols.Frames.FrameServices;
using snmpMon.Services;
using snmpMon.Services.Agent;
using snmpMon.Services.AlertServices;
using snmpMon.Services.SensorServices;
using snmpMon.Services.WindowServices;
using snmpMon.View;
using snmpMon.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using InterfaceOverview = snmpMon.View.InterfaceOverview;

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
            // Under Test
            service.AddTransient<IInterfacesSensors, InterfacesSensors>();
            service.AddTransient<IDeviceAvailabilityChecker, DeviceAvailabilityChecker>();
            service.AddTransient<ITemplateSevices, TemplateSevices>();
            service.AddTransient<IManageTemplateServices, ManageTemplateServices>();
            ///////
            service.AddTransient<IDeviceInterfacesServices, DeviceInterfacesServices>();
            service.AddTransient<NewTemplateViewModel>();
            service.AddTransient<MainViewModel>();
            service.AddSingleton<IDevicesServices, DevicesServices>();
            service.AddTransient<SnmpAlertsViewModel>();
            service.AddSingleton<ViewModels.CardViewModels.CardsViewModel>();
            service.AddTransient<IFremeTransientRequestServices, FremeTransientRequestServices>();
            service.AddTransient<AddDeviceVM>();
            service.AddTransient<AddDevicWin>();
            service.AddTransient<VerificationViewModel>();
            service.AddTransient<IWindowService, DeviceWindowService>();
            service.AddTransient<IDeviceRepository, DeviceRepository>();
            service.AddTransient<DeviceMonitoringWindow>();
            service.AddTransient<DeviceMonitoringVM>();
            service.AddTransient<IDashboardSensors, DashboardSensors>();
            service.AddTransient<ConfirmDialogVM>();
            // Test
            service.AddTransient<IMonitoringAgentServices,  MonitoringAgentServices>();
            /////////
            service.AddSingleton<IFrameRequestServices,  FrameRequestServices>();
            // Alert Notify and Devices VM
            service.AddSingleton<IAlertNotify, AlertNotify>();
            service.AddSingleton<DevicesVM>();
            ////////////////////////////////////////////////
            service.AddTransient<AlertViewModel>();
            service.AddSingleton<IRouterWindowMonitor, RouterWindowMonitor>();
            service.AddSingleton<IDeviceVerficationServices, DeviceVerficationServices>();
            service.AddTransient<IOverviewServices, OverviewServices>();
            service.AddTransient<UptimeDowntineStreamVM>();
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
            service.AddTransient<ISnmpTranslator, SnmpTranslator>();

            // Sub Projects 
            service.AddDatabaseServices();
            service.AddTransient<InterfaceOverview>();
            service.AddTransient<InterfaceOverviewViewModel>();
        }


        protected override void OnExit(ExitEventArgs e)
        {
            Environment.Exit(0);
            base.OnExit(e);
        }
    }
}
