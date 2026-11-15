using Microsoft.Extensions.DependencyInjection;
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
    /// <summary>
    /// Interaction logic for DeviceTemplates.xaml
    /// </summary>
    public partial class DeviceTemplates : UserControl
    {
        public DeviceTemplates()
        {
            InitializeComponent();
            this.DataContext = App.serviceProvider.GetRequiredService<NewTemplateViewModel>();
            TemplatesView.DataContext = App.serviceProvider.GetRequiredService<TemplatesViewModel>();
        }
    }
}
