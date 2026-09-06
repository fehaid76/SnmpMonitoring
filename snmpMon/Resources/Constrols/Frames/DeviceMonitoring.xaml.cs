using Microsoft.Extensions.DependencyInjection;
using snmpMon.Models;
using snmpMon.ViewModels;
using System;
using System.Collections.Generic;
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

namespace snmpMon.Resources.Constrols.Frames
{

    public partial class DeviceMonitoring : UserControl
    {

        public DeviceMonitoring()
        {
            InitializeComponent();
            this.Unloaded += DeviceMonitoring_Unloaded;
        }

        private void DeviceMonitoring_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= DeviceMonitoring_Unloaded;
            if (DataContext is DeviceMonitoringVM vm)
            {
                vm.Dispose();
            }
        }
    }
}
