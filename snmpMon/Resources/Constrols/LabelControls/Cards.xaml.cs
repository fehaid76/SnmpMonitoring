using LiveCharts.Wpf;
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

namespace snmpMon.Resources.Constrols.LabelControls
{
    /// <summary>
    /// Interaction logic for Cards.xaml
    /// </summary>
    public partial class Cards : UserControl
    {
        public Cards()
        {
            InitializeComponent();
            this.DataContext = App.serviceProvider.GetRequiredService<CardsViewModel>();
            var AlertVM = App.serviceProvider.GetRequiredService<AlertViewModel>();
            AlertVM.IsGeneric = true;
            AlertBox.DataContext = AlertVM;
        }

        
    }
}
