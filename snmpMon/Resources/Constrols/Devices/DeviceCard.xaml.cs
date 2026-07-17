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

namespace snmpMon.Resources.Constrols.Devices
{
    /// <summary>
    /// Interaction logic for DeviceCard.xaml
    /// </summary>

    public partial class DeviceCard : UserControl
    {

        public DeviceCard()
        {
            InitializeComponent();



        }

        public static readonly DependencyProperty DeviceNameProperty =
            DependencyProperty.Register("DeviceName", typeof(string), typeof(DeviceCard), new PropertyMetadata(string.Empty));

        public string DeviceName
        {
            get { return (string)GetValue(DeviceNameProperty); }
            set { SetValue(DeviceNameProperty, value); }
        }

        public static readonly DependencyProperty WarnningCountPropertyy =
            DependencyProperty.Register("WarnningCount", typeof(int), typeof(DeviceCard), new PropertyMetadata(default));

        public int WarnningCount
        {
            get => (int)GetValue(WarnningCountPropertyy);
            set
            {
                SetValue(WarnningCountPropertyy, value);
            }
        }

        public static readonly DependencyProperty ErrorCountProperty =
            DependencyProperty.Register("ErrorCount", typeof(int), typeof(DeviceCard), new PropertyMetadata(default));

        public int ErrorCount
        {
            get => (int)GetValue(ErrorCountProperty);
            set
            {
                SetValue(ErrorCountProperty, value);
            }
        }

        public static readonly DependencyProperty SuccessCountProperty =
            DependencyProperty.Register("SuccessCount", typeof(int), typeof(DeviceCard), new PropertyMetadata(default));

        public int SuccessCount
        {
            get => (int)GetValue(SuccessCountProperty);
            set
            {
                SetValue(SuccessCountProperty, value);
            }
        }

        public static readonly DependencyProperty IPAddressProperty =
            DependencyProperty.Register("IPAddress", typeof(string), typeof(DeviceCard), new PropertyMetadata(string.Empty));

        public string IPAddress
        {
            get { return (string)GetValue(IPAddressProperty); }
            set
            {
                SetValue(IPAddressProperty, value);
            }
        }

        public static readonly DependencyProperty PortsCountProperty =
            DependencyProperty.Register("PortsCount", typeof(int), typeof(DeviceCard), new PropertyMetadata(default));

        public int PortsCount
        {
            get => (int)GetValue(PortsCountProperty);
            set
            {
                SetValue(PortsCountProperty, value);
            }
        }

        public static readonly DependencyProperty IsConnectedProperty =
            DependencyProperty.Register("IsConnected", typeof(bool), typeof(DeviceCard), new PropertyMetadata(default));

        public bool IsConnected
        {
            get { return (bool)GetValue(IsConnectedProperty); }
            set
            {
                SetValue(IsConnectedProperty, value);
            }



        }

        public static readonly DependencyProperty GoCommandProperty = 
            DependencyProperty.Register("GoCommand", typeof(ICommand), typeof(DeviceCard), new PropertyMetadata(default));

        public ICommand GoCommand
        {
            get { return (ICommand)GetValue(GoCommandProperty); }
            set { SetValue(GoCommandProperty, value); }
        }



        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(DeviceCard), new PropertyMetadata(default));



        public ICommand DeleteCommand
        {
            get { return (ICommand)GetValue(DeleteCommandProperty); }
            set { SetValue(DeleteCommandProperty, value); }
        }

        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register("DeleteCommand", typeof(ICommand), typeof(DeviceCard), new PropertyMetadata(default));



        public object DeleteCommandParameter
        {
            get { return (object)GetValue(DeleteCommandParameterProperty); }
            set { SetValue(DeleteCommandParameterProperty, value); }
        }

        public static readonly DependencyProperty DeleteCommandParameterProperty =
            DependencyProperty.Register("DeleteCommandParameter", typeof(object), typeof(DeviceCard), new PropertyMetadata(default));






        public string Vendor
        {
            get { return (string)GetValue(VendorProperty); }
            set { SetValue(VendorProperty, value); }
        }

        public static readonly DependencyProperty VendorProperty =
            DependencyProperty.Register("Vendor", typeof(string), typeof(DeviceCard), new PropertyMetadata(string.Empty));









    }
}
