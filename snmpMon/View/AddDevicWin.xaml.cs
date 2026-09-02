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
using System.Windows.Shapes;

namespace snmpMon.View
{
    /// <summary>
    /// Interaction logic for AddDevicWin.xaml
    /// </summary>
    public partial class AddDevicWin : Window
    {
        
        public AddDevicWin()
        {
            InitializeComponent();
            this.DataContext = App.serviceProvider.GetRequiredService<AddDeviceVM>();
            
            if(this.DataContext is AddDeviceVM addDeviceVM && addDeviceVM != null)
                addDeviceVM.RequestCloseEvent += AddDeviceVM_RequestCloseEvent;


        }

        private void AddDeviceVM_RequestCloseEvent()
        {
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
