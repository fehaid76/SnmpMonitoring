using Microsoft.Extensions.DependencyInjection;
using snmpMon.ViewModel;
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

namespace snmpMon.Resources.Constrols.BoxesControls
{
    /// <summary>
    /// Interaction logic for UptimeDownstreamMonitor.xaml
    /// </summary>
    public partial class UptimeDownstreamMonitor : UserControl
    {
        public UptimeDownstreamMonitor()
        {
            InitializeComponent();
            this.DataContext = App.serviceProvider.GetRequiredService<UptimeDowntineStreamVM>();
        }
    }
}
