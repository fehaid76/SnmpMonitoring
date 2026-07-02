using Lextm.SharpSnmpLib.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using snmpMon.Engine.Services.SnmpServices;
using snmpMon.MibJson.Services;
using snmpMon.Services.SensorServices;
using snmpMon.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private readonly IMibParser mibParser;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = App.serviceProvider.GetRequiredService<MainViewModel>();
            this.Closed += MainWindow_Closed;
            snmpListener = App.serviceProvider.GetRequiredService<ISnmpListener>();
            mibParser = App.serviceProvider.GetRequiredService<IMibParser>();
            mibParser.ParseMibJson();


            try
            {
                snmpListener.StartSnmpListener();

            }
            catch (System.Net.Sockets.SocketException sex)
            {
                Debug.WriteLine($"Socket Error Code: {sex.SocketErrorCode}");
                Debug.WriteLine($"Socket Error Message: {sex.Message}");
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
