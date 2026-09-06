using Lextm.SharpSnmpLib.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using snmpMon.Engine.Helper;
using snmpMon.Engine.Services.SnmpServices;
using snmpMon.Engine.Services.SnmpTraslatorServices;
using snmpMon.MibJson.Services;
using snmpMon.Services;
using snmpMon.Services.SensorServices;
using snmpMon.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace snmpMon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ISnmpListener snmpListener;
        private readonly ISnmpSeverityAnalyzer snmpSeverityAnalyzer;
        private readonly IMibParser mibParser;
        private readonly IDevicesServices devicesServices;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = App.serviceProvider.GetRequiredService<MainViewModel>();
            this.Closed += MainWindow_Closed;
            snmpListener = App.serviceProvider.GetRequiredService<ISnmpListener>();
            mibParser = App.serviceProvider.GetRequiredService<IMibParser>();
            snmpSeverityAnalyzer = App.serviceProvider.GetRequiredService<ISnmpSeverityAnalyzer>();
            devicesServices = App.serviceProvider.GetRequiredService<IDevicesServices>();
            this.Loaded += MainWindow_Loaded;




            try
            {
                snmpListener.StartSnmpListener();

            }
            catch (System.Net.Sockets.SocketException ex)
            {
                Logger.LogCriticalException(ex, ex.Message);
            }

            

        }

        private async Task<bool> CheckForConnection()
        {
            bool isSuccess = false;
            try
            {
                Ping ping = new Ping();
                var isPingSuccess = await ping.SendPingAsync("8.8.8.8", 1000);
                isSuccess = isPingSuccess.Status == IPStatus.Success;
            }
            catch (TimeoutException ex)
            {
                Logger.LogCriticalException(ex, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogCriticalException(ex, ex.Message);
            }

            return isSuccess;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!await CheckForConnection())
            {
                MessageBox.Show("Please check your internet connection and try again.");

                this.Close();
            }
            try
            {
                var paths = await devicesServices.GetAllTemplatePathsAsync();
                await mibParser.ParseMibJson(paths);
               // await snmpSeverityAnalyzer.StartSeverityLevelsMapping();
            }
            catch (Exception ex)
            {
                Logger.LogCriticalException(ex, ex.Message);
            }

            
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            var sensors = App.serviceProvider.GetServices<ISensor>();

            foreach (var sensor in sensors)
            {
                if (sensor is IDisposable disposableSensor)
                {
                    sensor.Dispose();
                }

            }

            snmpListener.StopSnmpListener();
            Environment.Exit(0);
        }

    }
}
